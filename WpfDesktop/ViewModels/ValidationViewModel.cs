using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Validator.Application.Addresses;
using Validator.Domain.Addresses;
using WpfDesktop.Services;
using Validator.Domain.MailingResponses.Services;
using Validator.Application.Mailings.Contracts;
using CommunityToolkit.Mvvm.Input;
using Validator.Domain.MailingResponses.Models;
using Validator.Application.Mailings.Services;

namespace WpfDesktop.ViewModels
{
    public partial class ValidationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string selectedFilePath;

        [ObservableProperty]
        private bool isFileSelected;

        [ObservableProperty] 
        private bool isProcessing;

        [ObservableProperty]
        private MailingResponse mailingResponse;

        [ObservableProperty]
        private List<ValidatedAddress> validatedAddresses;

        public ICommand UploadCommand => _uploadCommand;

        private readonly ApplicationState _state;
        private readonly IMailingResponseParser _responseParser;
        private readonly MergeAddressValidationService _mergeService;

        private readonly IRelayCommand _uploadCommand;
        public ValidationViewModel(ApplicationState state, IMailingResponseParser responseParser)
        {
            _state = state;
            _responseParser = responseParser;
            _mergeService = new MergeAddressValidationService();


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
                        MessageBox.Show("Please select a file to upload", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    //TODO: Verify that the file is an XML file


                    await using var stream = new FileStream(SelectedFilePath, FileMode.Open, FileAccess.Read);

                    MailingResponse = _responseParser.ParseResponse(stream);

                    _state.MailingResponse = MailingResponse;

                    _state.HasValidatedAddresses = true;


                    _state.ValidationResponse = _mergeService.Merge(_state.MailingRequest, _state.MailingResponse);

                    ValidatedAddresses = _state.ValidationResponse.ValidatedAddressList?.Where(a => a.Severity != "INFO").ToList() ?? new List<ValidatedAddress>();

                }
            }

            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access denied: {ex.Message}. Please ensure you have proper permissions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"File access error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Log the exception details
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            finally
            {
                IsProcessing = false;
            }
        }
    }
}
