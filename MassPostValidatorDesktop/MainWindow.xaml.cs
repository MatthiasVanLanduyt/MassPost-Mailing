using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;
using Validator.Application.Addresses;
using Validator.Domain.Addresses;

namespace MassPostValidatorDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AddressValidator _addressValidator;
        private List<AddressLine> _addressLines = [];
        public MainWindow(IPostalCodeService postalCodeService)
        {
            InitializeComponent();
            _addressValidator = new AddressValidator(postalCodeService);
        }

        private void UploadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
            };

           

            if (openFileDialog.ShowDialog() == true)
            {
                string csvFile = openFileDialog.FileName;

                Console.WriteLine($"Selected file: {csvFile}");

                if (string.IsNullOrEmpty(csvFile))
                {
                    MessageBox.Show("Please select a file to upload", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //TODO: Verify that the file is a CSV file

                try
                {

                    using var stream = new FileStream(csvFile, FileMode.Open, FileAccess.Read);


                    _addressLines = CsvAddressParser.ReadCsvFile(stream).ToList();
                    //Show messagebox with number of address lines read
                    MessageBox.Show($"Successfully read {_addressLines.Count} address lines from file {openFileDialog.FileName}");

                    FileContentGrid.ItemsSource = _addressLines;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //    var lines = File.ReadAllLines(openFileDialog.FileName);
                //    var data = new List<dynamic>();

                //    foreach (var line in lines)
                //    {
                //        var values = line.Split(',');
                //        data.Add(new { Column1 = values[0], Column2 = values.Length > 1 ? values[1] : string.Empty });
                //    }

                //    myDataGrid.ItemsSource = data;
                //}

            }
        }

        private async void ValidateDataBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_addressLines.Count > 0)
            {
                await _addressValidator.ValidateAddressLinesAsync(_addressLines);
                ErrorGrid.ItemsSource = _addressLines.Where(x => x.Validation.Errors.Count > 0);
                FileContentGrid.Items.Refresh(); // Refresh the grid to show validation results
            }
        }
    }
}