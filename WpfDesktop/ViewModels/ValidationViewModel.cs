using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Validator.Application.Mailings.Contracts;
using Validator.Application.Mailings.Services;
using Validator.Domain.MailingResponses.Models;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
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

        private readonly ApplicationState _state;
        private readonly IMailingResponseParser _responseParser;
        private readonly ISnackbarService _snackbarService;
        private readonly MergeAddressValidationService _mergeService;

        private readonly IRelayCommand _uploadCommand;
        public ValidationViewModel(ApplicationState state, IMailingResponseParser responseParser, ISnackbarService snackbarService)
        {
            _state = state;
            _responseParser = responseParser;
            _snackbarService = snackbarService;
            _mergeService = new MergeAddressValidationService();
            validatedAddresses = new List<ValidatedAddress>();
            addressResponses = new List<AddressResponse>();


            _uploadCommand = new AsyncRelayCommand(UploadFileAsync);
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

                    _state.HasValidatedAddresses = true;

                    _state.HasUploadedResponseFile = true;

                    if (_state.HasGeneratedMailingRequest == false)
                    {
                        _state.ValidationResponse = _mergeService.Merge(_state.MailingRequest, _state.MailingResponse);
                        
                        ValidatedAddresses = _state.ValidationResponse.ValidatedAddressList?.Where(a => a.Severity != "INFO").ToList() ?? new List<ValidatedAddress>();
                    }

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
    }
}
