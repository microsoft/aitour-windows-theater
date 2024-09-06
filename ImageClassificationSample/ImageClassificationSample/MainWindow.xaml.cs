using System;
using System.Threading;
using Microsoft.UI.Xaml;

namespace ResNet_Image_ClassificationSample
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.RootFrame.Loaded += async (sender, args) =>
            {
                var sampleLoadingCts = new CancellationTokenSource();

                var localModelDetails = new SampleNavigationParameters(sampleLoadingCts.Token);

                RootFrame.Navigate(typeof(ImageClassificationResNet), localModelDetails);

                if (localModelDetails.ShouldWaitForCompletion)
                {
                    await localModelDetails.SampleLoadedCompletionSource.Task;
                }

                ProgressRingGrid.Visibility = Visibility.Collapsed;
            };
        }
    }
}
