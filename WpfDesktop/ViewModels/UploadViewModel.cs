using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Sharedkernel;
using Validator.Application.Addresses;
using Validator.Domain.Mailings.Models;
using Validator.Domain.Mailings.Services;
using Wpf.Ui;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Services;
using INavigationService = WpfDesktop.Contracts.Services.INavigationService;

namespace WpfDesktop.ViewModels
{
    public partial class UploadViewModel: ObservableObject
    {
        // Required Information Properties
        
        [ObservableProperty]
        private string accountID = "73771";

        [ObservableProperty]
        private string sequenceNumber = "01";

        [ObservableProperty]
        private string mailingReference;

        [ObservableProperty]
        private string mailFormat = "Small";

        [ObservableProperty]
        private DateTime expectedDelivery = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private string depositType = "tempDepositRef";

        [ObservableProperty]
        private string depositIdentifier;

        // File Upload Properties
        [ObservableProperty]
        private string selectedFilePath;

        [ObservableProperty]
        private bool isFileSelected;

        // Collection Properties for ComboBoxes
        public List<string> MailFormatOptions { get; } = new() { MailFormats.SmallFormat,MailFormats.LargeFormat };
        public List<string> DepositTypeOptions { get; } = new() { "depositRef", "tempDepositRef", "N" };
        
        // Commands

        private readonly AsyncRelayCommand _uploadCommand;
        private readonly AsyncRelayCommand _processCommand;
        private readonly RelayCommand _cancelCommand;

        public ICommand UploadCommand => _uploadCommand;
        public ICommand ProcessCommand => _processCommand;
        public ICommand CancelCommand => _cancelCommand;

        private readonly INavigationService _navigationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ApplicationState _state;
        private readonly IContactService _contactService;
        private readonly ISettingsService _settingsService;
        private readonly ISnackbarService _snackbarService;

        public UploadViewModel(
            ApplicationState state,
            IDateTimeProvider dateTimeProvider,
            INavigationService navigationService,
            IContactService contactService,
            ISettingsService settingsService,ISnackbarService snackbarService)
        {
            _state = state;
            _navigationService = navigationService;
            _dateTimeProvider = dateTimeProvider;
            _contactService = contactService;
            _settingsService = settingsService;
            _snackbarService = snackbarService;



            _uploadCommand = new AsyncRelayCommand(UploadFileAsync);
            _processCommand = new AsyncRelayCommand(ProcessFileAsync, CanProcessFile);
            _cancelCommand = new RelayCommand(Cancel);
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
                    _processCommand.NotifyCanExecuteChanged();

                    if (string.IsNullOrEmpty(SelectedFilePath))
                    {
                        MessageBox.Show("Please select a file to upload", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    //TODO: Verify that the file is a CSV file

                    using var stream = new FileStream(SelectedFilePath, FileMode.Open, FileAccess.Read);

                    _state.AddressList = CsvAddressParser.ReadCsvFile(stream).ToList();

                    _state.HasUploadedAddressList = true;

                    //TODO: Show messagebox with number of address lines read
                    MessageBox.Show($"Successfully read {_state.AddressCount} address lines from file {openFileDialog.FileName}");
                    
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
            try
            {
                var settings = _settingsService.GetMailingSettings();
                // Create a request
                var requestHeader = new MailIdRequestHeader
                {
                    SenderId = int.Parse(settings.SenderId),
                    AccountId = int.Parse(AccountID),
                    MailingRef = MailingReference,
                    ExpectedDeliveryDate = DateOnly.FromDateTime((DateTime)ExpectedDelivery),
                    CustomerBarcodeId = 530, // Consider making this configurable
                    CustomerFileRef = $"{_dateTimeProvider.DateStamp}MM",
                    DepositType = DepositType,
                    DepositIdentifier = DepositIdentifier,
                };

                // Create a factory with your bpost customer barcode ID
                var factory = new MailIdFactory(requestHeader.CustomerBarcodeId, int.Parse(SequenceNumber), _dateTimeProvider.DayOfTheYear); // Your 5-digit code from bpost
                
                var contacts = _contactService.GetContacts();

                var options = new MailIdOptions
                {
                    Mode = settings.Mode,
                    GenMid = settings.GenerateMailIds,
                    GenPsc = settings.GeneratePreSorting
                };

                var request = new MailIdRequest
                {
                    Header = requestHeader,
                    Options = options,
                    MailFormat = MailFormat,
                    MailFileInfo = MailingTypes.MailId,
                    Contacts = contacts
                };

                string priority = settings.Priority;
                string language = settings.AddressLanguage;

                // Convert your addresses
                foreach (var addressLine in _state.AddressList)
                {
                    var mailIdItem = factory.CreateFromAddress(addressLine, language: language, priority: priority);
                    request.Items.Add(mailIdItem);
                }

                _state.MailingRequest = request;

                _state.HasGeneratedMailingRequest = true;

                _navigationService.NavigateTo(typeof(HomeViewModel).FullName);

            }

            catch (Exception ex)
            {
                // Handle error appropriately
                // Show error message to user
            }
        }

        private bool CanProcessFile()
        {
            var fileSelected = IsFileSelected;
            var inputValid = ValidateInput();

            return fileSelected && inputValid;
        }

        private bool ValidateInput()
        {
            return !string.IsNullOrEmpty(AccountID) &&
                   !string.IsNullOrEmpty(SequenceNumber) &&
                   !string.IsNullOrEmpty(DepositIdentifier);
        }

        private void Cancel()
        {
            _navigationService.GoBack();
        }
    }
}
