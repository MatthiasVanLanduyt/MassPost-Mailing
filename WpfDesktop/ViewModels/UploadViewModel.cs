using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Sharedkernel;
using Validator.Application.Addresses;
using Validator.Application.Defaults;
using Validator.Application.Files;
using Validator.Application.Mailings.Contracts;
using Validator.Application.Mailings.Services;
using Validator.Domain.Addresses;
using Validator.Domain.Mailings.Models;
using Validator.Domain.Mailings.Services;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Views.Pages;

namespace WpfDesktop.ViewModels
{
    public class UploadViewModel: ObservableObject
    {
        public UploadViewModel() { }
        public string Title => "Upload Page";

        //TODO Put state somewhere else
        private List<AddressLine> _addressLines;

        // Required Information Properties
        [ObservableProperty]
        private string senderID = "4493";

        [ObservableProperty]
        private string accountID = "73771";

        [ObservableProperty]
        private string sequenceNumber = "01";

        [ObservableProperty]
        private string mailingReference;

        [ObservableProperty]
        private string mailFormat = "Small";

        [ObservableProperty]
        private string priority = "NP";

        [ObservableProperty]
        private string addressLanguage = "nl";

        [ObservableProperty]
        private DateTime expectedDelivery = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private string depositType = "depositRef";

        [ObservableProperty]
        private string depositIdentifier;

        // File Generation Options
        [ObservableProperty]
        private string outputFormat = "XML";

        [ObservableProperty]
        private string mode = "Test";

        [ObservableProperty]
        private bool generateMailIds = false;

        [ObservableProperty]
        private bool generatePreSorting = true;

        [ObservableProperty]
        private string sortingMode = "Customer Order (CU)";

        // File Upload Properties
        [ObservableProperty]
        private string selectedFilePath;

        [ObservableProperty]
        private bool isFileSelected;

        // Collection Properties for ComboBoxes
        public List<string> MailFormatOptions { get; } = new() { "Small", "Large" };
        public List<string> PriorityOptions { get; } = new() { "NP", "P" };
        public List<string> LanguageOptions { get; } = new() { "nl", "fr", "en" };
        public List<string> DepositTypeOptions { get; } = new() { "depositRef", "tempDepositRef", "N" };
        public List<string> OutputFormatOptions { get; } = new() { "XML", "TXT" };
        public List<string> ModeOptions { get; } = new() { "Test", "Production" };
        public List<string> SortingModeOptions { get; } = new() { "Customer Order (CU)", "Print Order (PO)" };

        // Commands
        public ICommand UploadCommand => new AsyncRelayCommand(UploadFileAsync);
        public ICommand ProcessCommand => new AsyncRelayCommand(ProcessFileAsync, CanProcessFile);
        public ICommand CancelCommand => new RelayCommand(Cancel);

        private readonly INavigationService _navigationService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UploadViewModel(
            IDateTimeProvider dateTimeProvider,
            INavigationService navigationService)
        {
            _navigationService = navigationService;
            _dateTimeProvider = dateTimeProvider;
        }

        private async Task UploadFileAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFilePath = openFileDialog.FileName;
                IsFileSelected = true;
                
                if (string.IsNullOrEmpty(SelectedFilePath))
                {
                    MessageBox.Show("Please select a file to upload", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                    //TODO: Verify that the file is a CSV file

                    using var stream = new FileStream(SelectedFilePath, FileMode.Open, FileAccess.Read);

                    _addressLines = CsvAddressParser.ReadCsvFile(stream).ToList();
                    
                    //Show messagebox with number of address lines read
                    MessageBox.Show($"Successfully read {_addressLines.Count} address lines from file {openFileDialog.FileName}");

                    CommandManager.InvalidateRequerySuggested();
            }

           


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async Task ProcessFileAsync()
        {
            if (!ValidateInput())
            {
                // Show validation error
                return;
            }

            try
            {
                MailIdOptions mailIdOptions = new()
                {
                    Mode = Mode,
                    GenMid = GenerateMailIds ? "Y" : "N",
                    GenPSC = GeneratePreSorting ? "Y" : "N",
                };

                // Create a request
                var requestHeader = new MailIdRequestHeader
                {

                    SenderId = int.Parse(SenderID),
                    AccountId = int.Parse(AccountID),
                    MailingRef = MailingReference,
                    ExpectedDeliveryDate = DateOnly.FromDateTime((DateTime)ExpectedDelivery),
                    CustomerBarcodeId = 530, // Consider making this configurable
                    CustomerFileRef = $"{_dateTimeProvider.DateStamp}MM",
                    DepositType = DepositType,
                    DepositIdentifier = DepositIdentifier,
                };

                var sequenceNumber = int.Parse(SequenceNumber);
                // Create a factory with your bpost customer barcode ID
                var factory = new MailIdFactory(requestHeader.CustomerBarcodeId, sequenceNumber, _dateTimeProvider.DayOfTheYear); // Your 5-digit code from bpost

                var request = new MailIdRequest
                {
                    Header = requestHeader,
                    Options = mailIdOptions,
                    MailFormat = MailFormat,
                    MailFileInfo = MailingTypes.MailId,
                    Contacts = DefaultContacts.GetDefaults().ToList(),
                };

                string priority = Priority;
                string language = AddressLanguage;

                // Convert your addresses
                foreach (var addressLine in _addressLines)
                {
                    var mailIdItem = factory.CreateFromAddress(addressLine, language: language, priority: priority);
                    request.Items.Add(mailIdItem);
                }

                var generator = OutputFormat == "XML" ?
                    new XmlMailIdFileGenerator(_dateTimeProvider) :
                    new TxtMailIdFileGenerator(_dateTimeProvider) as IMailIdFileGenerator;

                var file = generator.GenerateFile(request);
                var savedFile = FileOperations.SaveFile(file, @"C:\Users\vanlanm\Downloads");
                FileOperations.OpenFile(savedFile.FullName);

                _navigationService.NavigateTo(typeof(UploadViewModel).FullName);
                
    }

            catch (Exception ex)
            {
                // Handle error appropriately
                // Show error message to user
            }
        }

        private bool CanProcessFile()
        {
            return IsFileSelected && ValidateInput();
        }

        private bool ValidateInput()
        {
            return !string.IsNullOrEmpty(SenderID) &&
                   !string.IsNullOrEmpty(AccountID) &&
                   !string.IsNullOrEmpty(SequenceNumber) &&
                   !string.IsNullOrEmpty(DepositIdentifier);
        }

        private void Cancel()
        {
            _navigationService.GoBack();
        }
    }
}
