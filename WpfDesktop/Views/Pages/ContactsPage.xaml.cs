using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui;
using WpfDesktop.ViewModels;

namespace WpfDesktop.Views.Pages
{
    /// <summary>
    /// Interaction logic for ContactsPage.xaml
    /// </summary>
    public partial class ContactsPage : Page
    {
        public ISnackbarService SnackbarService { get; }

        public ContactsPage(ContactsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
          
           
        }
    }
}
