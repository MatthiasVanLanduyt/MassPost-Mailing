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

namespace WpfDesktop.ViewModels
{
    public partial class ValidationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string selectedFilePath;

        [ObservableProperty]
        private bool isFileSelected;

        [ObservableProperty]
        private MailingResponse mailingResponse;

        public ICommand UploadCommand => _uploadCommand;

        private readonly ApplicationState _state;
        private readonly IMailingResponseParser _responseParser;

        private readonly IRelayCommand _uploadCommand;
        public ValidationViewModel(ApplicationState state, IMailingResponseParser responseParser)
        {
            _state = state;
            _responseParser = responseParser;

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

                    if (string.IsNullOrEmpty(SelectedFilePath))
                    {
                        MessageBox.Show("Please select a file to upload", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    //TODO: Verify that the file is an XML file

                   
                    
                    using var stream = new FileStream(SelectedFilePath, FileMode.Open, FileAccess.Read);

                    MailingResponse = _responseParser.ParseResponse(stream);

                    _state.HasValidatedAddresses = true;



                    //CommandManager.InvalidateRequerySuggested();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
