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
using Wpf.Ui.Controls;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Services;
using INavigationService = WpfDesktop.Contracts.Services.INavigationService;
using Wpf.Ui.Extensions;
using System.Net;

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
        private string mailFormat = MailFormats.SmallFormat;

        [ObservableProperty]
        private DateTime expectedDelivery = DateTime.Now.AddDays(1);

        [ObservableProperty]
        private string depositType = DepositIdentifierTypes.TemporaryDepositReference;

        [ObservableProperty]
        private string depositIdentifier = string.Empty;

        // File Upload Properties
        [ObservableProperty]
        private string? selectedFilePath;

        [ObservableProperty]
        private bool isFileSelected;

        // Collection Properties for ComboBoxes
        public List<string> MailFormatOptions { get; } = [MailFormats.SmallFormat, MailFormats.LargeFormat];
        public List<string> DepositTypeOptions { get; } =
        [
            DepositIdentifierTypes.TemporaryDepositReference, DepositIdentifierTypes.DepositReference,
            DepositIdentifierTypes.NoDeposit
        ];
        
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
                       _snackbarService.Show(
                           "No file selected",
                           $"Please select a file to upload",
                           ControlAppearance.Caution,
                           new TimeSpan(0, 0, 3));
                        
                   }

                   //TODO: Verify that the file is a CSV file

                   using var stream = new FileStream(SelectedFilePath, FileMode.Open, FileAccess.Read);

                   _state.AddressList = CsvAddressParser.ReadCsvFile(stream).ToList();

                   _state.HasUploadedAddressList = true;

                   _snackbarService.Show(
                       "Address file uploaded",
                       $"Successfully read { _state.AddressCount} address lines from file {SelectedFilePath}",
                       ControlAppearance.Success,
                       new TimeSpan(0, 0, 3));

                   CommandManager.InvalidateRequerySuggested();
               }

           }
           catch (Exception ex)
           {
               _snackbarService.Show(
                   "Address file upload failed",
                   $"Error reading file: {ex.Message}",
                   ControlAppearance.Danger,
                   new TimeSpan(0, 0, 5));
                
           }

        }

        private async Task ProcessFileAsync()
        {
            var validation = ValidateInput();

            if (!validation)
            {
                _snackbarService.Show(
                    "Invalid input",
                    "Please provide all required information",
                    ControlAppearance.Caution,
                    new TimeSpan(0, 0, 3));

                return;
            }

            try
            {
                var settings = _settingsService.GetMailingSettings();
                // Create a request
                var requestHeader = new MailIdRequestHeader
                {
                    SenderId = int.Parse(settings.SenderId),
                    AccountId = int.Parse(AccountID),
                    MailingRef = $"{_dateTimeProvider.DateStamp}{SequenceNumber}-01",
                    ExpectedDeliveryDate = DateOnly.FromDateTime((DateTime)ExpectedDelivery),
                    CustomerBarcodeId = 530, // Consider making this configurable
                    CustomerFileRef = $"{_dateTimeProvider.DateStamp}{SequenceNumber}",
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

                _navigationService.NavigateTo(typeof(HomeViewModel).FullName ?? string.Empty);

                _snackbarService.Show(
                    "Mailing request generated",
                    $"Mailing request is available to download",
                    ControlAppearance.Success,
                    new TimeSpan(0, 0, 3));

            }

            catch (Exception ex)
            {
                _snackbarService.Show(
                    "Failed to generate mailing request",
                    $"Error generating mailing request: \n{ex.Message}",
                    ControlAppearance.Danger,
                    new TimeSpan(0, 0, 3));
            }
        }

        private bool CanProcessFile()
        {
            return IsFileSelected;
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
