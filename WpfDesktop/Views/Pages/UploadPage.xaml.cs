using System.Windows.Controls;
using WpfDesktop.ViewModels;

namespace WpfDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for UploadPage.xaml
    /// </summary>
    public partial class UploadPage : Page
    {
        public UploadPage(UploadViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
