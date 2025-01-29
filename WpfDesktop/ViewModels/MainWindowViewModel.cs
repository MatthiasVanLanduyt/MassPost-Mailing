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
        public string ApplicationTitle { get => "MassPost Application"; }

        private INavigationService _navigationService;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand NavigateToHomeCommand => new RelayCommand(() => NavigateTo(typeof(HomeViewModel)));

        public ICommand NavigateToSettingsCommand => new RelayCommand(() => NavigateTo(typeof(SettingsViewModel)));

        public ICommand NavigateToUploadCommand => new RelayCommand(() => NavigateTo(typeof(UploadViewModel)));

        public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(OnUnloaded));
        public MainWindowViewModel(INavigationService navigationService) 
        {
            _navigationService = navigationService;
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
            if (targetViewModel != null)
            {
                _navigationService.NavigateTo(targetViewModel.FullName);
            }
        }

        private void OnNavigated(object sender, string viewModelName)
        {
           

            
        }
    }
}
