using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Validator.Application.Files;
using Validator.Application.Mailings.Contracts;
using Validator.Application.Mailings.Services;
using Validator.Domain.MailingResponses.Models;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using WpfDesktop.Contracts.Services;
using WpfDesktop.Models;
using WpfDesktop.Services;

namespace WpfDesktop.ViewModels
{
    public partial class ValidationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? selectedFilePath;

        [ObservableProperty]
        private bool isFileSelected;

        [ObservableProperty] 
        private bool isProcessing;

        [ObservableProperty]
        private MailingResponse? mailingResponse;

        [ObservableProperty]
        private List<ValidatedAddress> validatedAddresses;

        [ObservableProperty]
        private List<AddressResponse> addressResponses;

        public ICommand UploadCommand => _uploadCommand;
        public ICommand DownloadCommand => _downloadCommand;

        private readonly ApplicationState _state;
        private readonly MailingSettings _settings;
        private readonly IMailingResponseParser _responseParser;
        private readonly ISnackbarService _snackbarService;
        private readonly MergeAddressValidationService _mergeService;

        private readonly IRelayCommand _uploadCommand;
        private readonly IRelayCommand _downloadCommand;
        public ValidationViewModel(ApplicationState state, ISettingsService settingsService, IMailingResponseParser responseParser, ISnackbarService snackbarService)
        {
            _state = state;
            _responseParser = responseParser;
            _snackbarService = snackbarService;
            _mergeService = new MergeAddressValidationService();
            validatedAddresses = new List<ValidatedAddress>();
            addressResponses = new List<AddressResponse>();

            _settings = settingsService.GetMailingSettings();


            _uploadCommand = new AsyncRelayCommand(UploadFileAsync);
            _downloadCommand = new RelayCommand(DownloadFile);
        }

        private async Task UploadFileAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*",
                    FilterIndex = 1
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    SelectedFilePath = openFileDialog.FileName;
                    IsFileSelected = true;
                    IsProcessing = true;

                    if (string.IsNullOrEmpty(SelectedFilePath))
                    {
                        _snackbarService.Show(
                            "No file selected",
                            $"Please select a file to upload",
                            ControlAppearance.Caution,
                            new TimeSpan(0, 0, 3));
                        
                    }

                    //TODO: Verify that the file is an XML file


                    await using var stream = new FileStream(SelectedFilePath, FileMode.Open, FileAccess.Read);

                    MailingResponse = _responseParser.ParseResponse(stream);

                    if (MailingResponse is null)
                    {
                        _snackbarService.Show(
                            "Upload response failed",
                            $"Failed to upload response file",
                            ControlAppearance.Danger,
                            new TimeSpan(0, 0, 3));
                    }

                    AddressResponses = MailingResponse!.AddressResponses;

                    _state.MailingResponse = MailingResponse;

                    _state.HasUploadedResponseFile = true;

                    _snackbarService.Show(
                            "Upload Successful",
                            $"Successfully uploaded response file",
                            ControlAppearance.Success,
                            new TimeSpan(0, 0, 3));
                }
            }

            catch (UnauthorizedAccessException ex)
            {
                _snackbarService.Show(
                    "Unauthorized",
                    $"Access denied: {ex.Message}. Please ensure you have proper permissions.",
                    ControlAppearance.Danger,
                    new TimeSpan(0, 0, 3));
            }
            catch (IOException ex)
            {
                _snackbarService.Show(
                    "File access error",
                    $"File access error: {ex.Message}",
                    ControlAppearance.Danger,
                    new TimeSpan(0, 0, 3));
                
            }
            catch (Exception ex)
            {
                _snackbarService.Show(
                    "Failed to validated",
                    $"An error occurred: {ex.Message}",
                    ControlAppearance.Danger,
                    new TimeSpan(0, 0, 3));
            }

            finally
            {
                IsProcessing = false;
            }
        }
        
        private void DownloadFile()
        {
            try
            {
                if (AddressResponses.Count == 0)
                {
                    _snackbarService.Show(
                        "No data to download",
                        $"No data to download",
                        ControlAppearance.Caution,
                        new TimeSpan(0, 0, 3));
                    return;
                }
                
                var generator = new MailingResponseCsvExporter();

                var file = generator.ExportToCsv(_state.MailingResponse!);

                var savedFile = FileOperations.SaveFile(file, _settings.DefaultSaveLocation);
                FileOperations.OpenFile(savedFile.FullName);
     
            }
            catch (Exception ex)
            {
                _snackbarService.Show(
                    "Failed to download",
                    $"An error occurred: {ex.Message}",
                    ControlAppearance.Danger,
                    new TimeSpan(0, 0, 3));
            }
        }
    }
}
