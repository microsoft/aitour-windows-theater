using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ResNet_Image_ClassificationSample
{
    internal class SampleNavigationParameters
    {
        private bool shouldWaitForCompletion;

        public CancellationToken CancellationToken { get; private set; }
        public string ModelPath => System.IO.Path.Join(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Models", @"resnet101-v1-7.onnx");
        
        public bool ShouldWaitForCompletion
        {
            [MemberNotNullWhen(true, nameof(SampleLoadedCompletionSource))]
            get
            {
                return shouldWaitForCompletion;
            }
        }

        public TaskCompletionSource<bool>? SampleLoadedCompletionSource { get; set; }

        public SampleNavigationParameters(CancellationToken loadingCanceledToken)
        {
            CancellationToken = loadingCanceledToken;
        }

        public void RequestWaitForCompletion()
        {
            shouldWaitForCompletion = true;
            SampleLoadedCompletionSource = new TaskCompletionSource<bool>();
        }

        public void NotifyCompletion()
        {
            SampleLoadedCompletionSource?.SetResult(true);
        }
    }
}