using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfDesktop.Contracts.Services;

namespace WpfDesktop.ViewModels
{
    public class MainWindowViewModel: ObservableObject
    {
        public string ApplicationTitle => "MassPost Application";

        private INavigationService _navigationService;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;

        public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

        public ICommand NavigateToHomeCommand => new RelayCommand(() => NavigateTo(typeof(HomeViewModel)));

        public ICommand NavigateToSettingsCommand => new RelayCommand(() => NavigateTo(typeof(SettingsViewModel)));

        public ICommand NavigateToUploadCommand => new RelayCommand(() => NavigateTo(typeof(UploadViewModel)));

        public ICommand NavigateToValidationCommand => new RelayCommand(() => NavigateTo(typeof(ValidationViewModel)));

        public ICommand NavigateToContactsCommand => new RelayCommand(() => NavigateTo(typeof(ContactsViewModel)));

        public ICommand UnloadedCommand => _unloadedCommand ??= new RelayCommand(OnUnloaded);
        public MainWindowViewModel(INavigationService navigationService) 
        {
            _navigationService = navigationService;
            _loadedCommand = new RelayCommand(OnLoaded);
            _unloadedCommand = new RelayCommand(OnUnloaded);
        }

        private void OnLoaded()
        {
            _navigationService.Navigated += OnNavigated;
        }

        private void OnUnloaded()
        {
            _navigationService.Navigated -= OnNavigated;
        }

        private bool CanGoBack()
            => _navigationService.CanGoBack;

        private void OnGoBack()
            => _navigationService.GoBack();

        private void NavigateTo(Type targetViewModel)
        {
            if (!string.IsNullOrEmpty(targetViewModel.FullName))
            {
                _navigationService.NavigateTo(targetViewModel.FullName);
            }
        }

        private void OnNavigated(object sender, string viewModelName)
        {
           

            
        }
    }
}
