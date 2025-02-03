using System.Formats.Tar;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Sharedkernel;
using Validator.Application.Addresses;
using Validator.Application.Defaults;
using Validator.Application.Files;
using Validator.Application.Mailings;
using Validator.Application.Mailings.Contracts;
using Validator.Application.Mailings.Models;
using Validator.Application.Mailings.Services;
using Validator.Domain.Addresses;
using Validator.Domain.Mailings.Models;
using Validator.Domain.Mailings.Services;

namespace Validator.Desktop
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
            OptionsExpander.Visibility = Visibility.Visible;
            OptionsExpander.IsExpanded = false;

            MailRequestExpander.Visibility = Visibility.Visible;
            MailRequestExpander.IsExpanded = false;

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
            
            MailIdOptions mailIdOptions = new()
            {
                Mode = ModeComboBox.SelectedItem.ToString(),
                GenMid = GenerateMailIdsComboBox.SelectedItem.ToString(),
                GenPSC = GeneratePresortingComboBox.SelectedItem.ToString()
            };

            // Create a request
            var requestHeader = new MailIdRequestHeader
            {
                
                SenderId = int.Parse(SenderIdTextBox.Text),
                AccountId = int.Parse(AccountIdTextBox.Text),
                MailingRef = MailingReferenceTextBox.Text,
                ExpectedDeliveryDate = DateOnly.FromDateTime((DateTime)DeliveryDatePicker.SelectedDate),
                CustomerBarcodeId = 530, // Consider making this configurable
                CustomerFileRef = $"{_dateTimeProvider.DateStamp}MM",                
                DepositType = DepositTypeComboBox.SelectedItem.ToString(),
                DepositIdentifier = DepositIdentifierTextBox.Text
            };

            var sequenceNumber = int.Parse(SequenceNumberTextBox.ToString());
            // Create a factory with your bpost customer barcode ID
            var factory = new MailIdFactory(requestHeader.CustomerBarcodeId, sequenceNumber, _dateTimeProvider.DayOfTheYear); // Your 5-digit code from bpost

            var request = new MailIdRequest
            {
                Header = requestHeader,
                Options = mailIdOptions,
                MailFormat = MailFormatComboBox.SelectedItem.ToString() == "Small" ?
            MailFormats.SmallFormat : MailFormats.LargeFormat,
                MailFileInfo = MailingTypes.MailId,
                Contacts = DefaultContacts.GetDefaults().ToList(),
            };

            string priority = PriorityComboBox.SelectedItem.ToString();
            string language = LanguageComboBox.SelectedItem.ToString();

            // Convert your addresses
            foreach (var addressLine in _addressLines)
            {
                var mailIdItem = factory.CreateFromAddress(addressLine, language: language, priority: priority);
                request.Items.Add(mailIdItem);
            }

            var generator = OutputFormatComboBox.SelectedItem.ToString() == MailListFileOutputs.XML ?
                new XmlMailIdFileGenerator(_dateTimeProvider) :
                new TxtMailIdFileGenerator(_dateTimeProvider) as IMailIdFileGenerator;
            
            var file = generator.GenerateFile(request);
            var savedFile = FileOperations.SaveFile(file, @"C:\Users\vanlanm\Downloads");
            FileOperations.OpenFile(savedFile.FullName);
        }
    }
}