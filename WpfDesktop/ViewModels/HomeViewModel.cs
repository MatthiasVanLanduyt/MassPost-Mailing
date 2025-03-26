using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sharedkernel;
using Validator.Application.Files;
using Validator.Application.Mailings.Contracts;
using Validator.Application.Mailings.Services;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Models;
using WpfDesktop.Services;

namespace WpfDesktop.ViewModels
{
    public partial class HomeViewModel: ObservableObject
    {

        private readonly INavigationService _navigationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private ApplicationState _state;

        public string UploadButtonAppearance => "Primary";

        public string DownloadMailingRequestButtonAppearance => "Primary";

        public string DownloadAddressListButtonAppearance => "Primary";

        public string ValidationButtonAppearance => "Primary";

        public string DownloadMailingResponseButtonAppearance => "Primary";

        private readonly MailingSettings _settings;

        public ICommand NavigateToUploadCommand => new RelayCommand(() => NavigateTo(typeof(UploadViewModel)));
        public ICommand NavigateToGenerateCommand => new RelayCommand(() => NavigateTo(typeof(UploadViewModel)));
        public ICommand NavigateToValidateCommand => new RelayCommand(() => NavigateTo(typeof(ValidationViewModel)));

        public ICommand DownloadMailAddressListCommand => _downloadMailAddressListCommand;
        public ICommand DownloadMailRequestCommand => _downloadMailRequestCommand;

        public ICommand DownloadMailResponseCommand => _downloadMailResponseCommand;

        private readonly ICommand _downloadMailAddressListCommand;
        private readonly ICommand _downloadMailRequestCommand;
        private readonly ICommand _downloadMailResponseCommand;

        public HomeViewModel(INavigationService navigationService, ApplicationState state, IDateTimeProvider dateTimeProvider, ISettingsService settingsService)
        {
            _navigationService = navigationService;
            _state = state;
            _dateTimeProvider = dateTimeProvider;
            _settingsService = settingsService;

            _settings = _settingsService.GetMailingSettings();

            _downloadMailRequestCommand = new RelayCommand(DownloadMailRequest);
            _downloadMailResponseCommand = new RelayCommand(DownloadMailResponseList);
            _downloadMailAddressListCommand = new RelayCommand(DownloadMailAddressList);
        }

        
        private void DownloadMailRequest()
        {
            var generator = _settings.OutputFormat == "XML" ?
                   new XmlMailIdFileGenerator(_dateTimeProvider) :
                   new TxtMailIdFileGenerator(_dateTimeProvider) as IMailIdFileGenerator;

            var file = generator.GenerateFile(State.MailingRequest!);
            var savedFile = FileOperations.SaveFile(file,_settings.DefaultSaveLocation);
            FileOperations.OpenFile(savedFile.FullName);

            State.HasDownloadedMailingRequest = true;
        }

        private void DownloadMailAddressList()
        {
            if (State.MailingRequest == null) return;

            var generator = new CsvMailIdAddressFileGenerator();

            var file = generator.GenerateFile(State.MailingRequest.Items, $"AddressList_{State.MailingRequest.Header.CustomerFileRef}.csv");

            var savedFile = FileOperations.SaveFile(file, _settings.DefaultSaveLocation);
            FileOperations.OpenFile(savedFile.FullName);

            State.HasDownloadedMailingAddressList = true;
        }

        private void DownloadMailResponseList()
        {
            var generator = new MailingResponseCsvExporter();

            var file = generator.ExportToCsv(State.MailingResponse!);

            var savedFile = FileOperations.SaveFile(file, _settings.DefaultSaveLocation);
            FileOperations.OpenFile(savedFile.FullName);
        }


        public string Title => "Home Page";

        private void NavigateTo(Type targetViewModel)
        {
            if (!string.IsNullOrEmpty(targetViewModel.FullName)) _navigationService.NavigateTo(targetViewModel.FullName);
        }
    }
}
