using System.Formats.Tar;
using System.IO;
using System.Windows;
using Sharedkernel;
using Validator.Application.Addresses;
using Validator.Application.Defaults;
using Validator.Application.Files;
using Validator.Application.Mailings;
using Validator.Application.Mailings.Services;
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
            
            var depositId = 1; 

            // Create a request
            var requestHeader = new MailIdRequestHeader
            {
                SenderId = 4493,
                AccountId = 73771,
                MailingRef = "Mailingman01",
                ExpectedDeliveryDate = _dateTimeProvider.DateNow,
                CustomerBarcodeId = 530,
                CustomerFileRef = $"{_dateTimeProvider.DateStamp}MM",
                Mode = "T"
            };

            // Create a factory with your bpost customer barcode ID
            var factory = new MailIdFactory(requestHeader.CustomerBarcodeId, depositId, _dateTimeProvider.DayOfTheYear); // Your 5-digit code from bpost

            var request = new MailIdRequest
            {
                Header = requestHeader,
                Options = new MailIdOptions
                {
                    DepositId = "",
                    DepositIdentifierType = "",
                    GenMid = "N",
                    GenPSC = "Y"
                },
                MailFormat = MailFormats.SmallFormat,
                MailFileInfo = MailingTypes.MailId,
                Contacts = DefaultContacts.GetDefaults().ToList(),
            };

            // Convert your addresses
            foreach (var addressLine in _addressLines)
            {
                var mailIdItem = factory.CreateFromAddress(addressLine);
                request.Items.Add(mailIdItem);
            }

            // Generate the file
            var generator = new XmlMailIdFileGenerator(_dateTimeProvider);
            var fileOps = new FileOperations();

            var file = generator.GenerateFile(request);
            var savedFile = fileOps.SaveFile(file, @"C:\Users\vanlanm\Downloads");
            fileOps.OpenFile(savedFile.FullName);
        }
    }
}