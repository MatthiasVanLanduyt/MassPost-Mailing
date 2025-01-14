using System.Formats.Tar;
using System.IO;
using System.Windows;
using Sharedkernel;
using Validator.Application.Addresses;
using Validator.Application.Files;
using Validator.Application.Mailings;
using Validator.Domain.Addresses;
using Validator.Domain.Mailings.Models;
using Validator.Domain.Mailings.Services;

namespace MassPostValidatorDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AddressValidator _addressValidator;
        private List<AddressLine> _addressLines = [];
        private MailIdSettings _settings;
        private IDateTimeProvider _dateTimeProvider;
        public MainWindow(IPostalCodeService postalCodeService, IDateTimeProvider dateTimeProvider)
        {
            InitializeComponent();
            _addressValidator = new AddressValidator(postalCodeService);
            _settings = MailIdSettings.LoadDefaults();
            _dateTimeProvider = dateTimeProvider;
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

                    EnablePostUploadUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EnablePostUploadUI()
        {
            // Show configuration
            ConfigExpander.Visibility = Visibility.Visible;
            ConfigExpander.IsExpanded = false;

            // Enable action buttons
            ValidateDataBtn.IsEnabled = true;
            GenerateMailingListBtn.IsEnabled = true;

            // Show the datagrids
            DatagridsTabs.Visibility = Visibility.Visible;
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

        private void GenerateMailingListBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create a factory with your bpost customer barcode ID
            var factory = new MailIdFactory("12345"); // Your 5-digit code from bpost

            // Create a request
            var requestHeader = new MailIdRequestHeader
            {
                CustomerId = 56,
                AccountId = 3,
                MailingRef = "test",
                ExpectedDeliveryDate = _dateTimeProvider.TimeStamp,
                SerialNumber = "12345",
            };

            var request = new MailIdRequest
            {
                Header = requestHeader
            };

            // Convert your addresses
            foreach (var addressLine in _addressLines)
            {
                var mailIdItem = factory.CreateFromAddress(addressLine);
                request.Items.Add(mailIdItem);
            }

            // Generate the file
            var generator = new MailIdFileGenerator(_dateTimeProvider);
            var fileOps = new FileOperations();

            var textFile = generator.GenerateTxtFile(request);
            var savedFile = fileOps.SaveFile(textFile, @"C:\Users\vanlanm\Downloads");
            fileOps.OpenFile(savedFile.FullName);
        }
    }
}