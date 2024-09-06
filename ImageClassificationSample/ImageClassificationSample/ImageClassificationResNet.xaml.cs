using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using System.IO;

namespace ResNet_Image_ClassificationSample
{
    internal sealed partial class ImageClassificationResNet : Page
    {
        private InferenceSession? _inferenceSession;

        public ImageClassificationResNet()
        {
            this.Unloaded += (s, e) => _inferenceSession?.Dispose();

            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is SampleNavigationParameters sampleParams)
            {
                sampleParams.RequestWaitForCompletion();
                await InitModel(sampleParams.ModelPath);
                sampleParams.NotifyCompletion();

                await ClassifyImage(Path.Join(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets", "team.jpg"));
            }
        }

        private Task InitModel(string modelPath)
        {
            return Task.Run(() =>
            {
                if (_inferenceSession != null)
                {
                    return;
                }

                SessionOptions sessionOptions = new SessionOptions();
                sessionOptions.RegisterOrtExtensions();

                _inferenceSession = new InferenceSession(modelPath, sessionOptions);
            });
        }

        private async void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Create a FileOpenPicker
            var picker = new FileOpenPicker();

            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            // Set the file type filter
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");

            picker.ViewMode = PickerViewMode.Thumbnail;

            // Pick a file
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Call function to run inference and classify image
                await ClassifyImage(file.Path);
            }
            else
            {
                PredictionsStackPanel.Children.Clear();

                TextBlock feedbackTextBlock = new TextBlock
                {
                    Text = "No image selected",
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                PredictionsStackPanel.Children.Add(feedbackTextBlock);
            }
        }

        private async Task ClassifyImage(string filePath)
        {
            if (!Path.Exists(filePath))
            {
                return;
            }

            // Display the selected image
            BitmapImage bitmapImage = new BitmapImage(new Uri(filePath));
            UploadedImage.Source = bitmapImage;

            var predictions = await Task.Run(() =>
            {
                Bitmap image = new Bitmap(filePath);

                // Resize image
                int width = 224;
                int height = 224;
                image = BitmapFunctions.ResizeBitmap(image, width, height);

                // Preprocess image
                Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, 224, 224 });
                input = BitmapFunctions.PreprocessBitmapWithStdDev(image, input);

                // Setup inputs
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor((string?)"data", input)
                };

                // Run inference
                using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _inferenceSession!.Run(inputs);

                // Postprocess to get softmax vector
                IEnumerable<float> output = results[0].AsEnumerable<float>();
                return ImageNet.GetSoftmax(output);
            });

            // Populates table of results
            ImageNet.DisplayPredictions(predictions, PredictionsStackPanel);
        }
    }
}