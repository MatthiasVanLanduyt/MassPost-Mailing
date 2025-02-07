using System.Windows;
using System.Windows.Controls;
using Wpf.Ui;
using Wpf.Ui.Controls;
using WpfDesktop.Contracts.Views;
using WpfDesktop.ViewModels;

namespace WpfDesktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow, IShellWindow
    {
        public MainWindowViewModel ViewModel { get; }
        public MainWindow(MainWindowViewModel viewModel, ISnackbarService snackbarService)
        {
            InitializeComponent();
            DataContext = viewModel;
            ViewModel = viewModel;
            snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        }

        public Frame GetNavigationFrame()
            => shellFrame;

        public void ShowWindow()
            => Show();

        public void CloseWindow()
            => Close();
    }
}