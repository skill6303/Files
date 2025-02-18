using Files.App.ViewModels.Previews;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.App.UserControls.FilePreviews
{
    public sealed partial class HtmlPreview : UserControl
    {
        // TODO: Move to WebView2 on WinUI 3.0 release

        public HtmlPreview(HtmlPreviewViewModel model)
        {
            ViewModel = model;
            InitializeComponent();
        }

        public HtmlPreviewViewModel ViewModel { get; set; }

        private void WebViewControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            WebViewControl.NavigateToString(ViewModel.TextValue);
        }
    }
}