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
    public class HomeViewModel: ObservableObject
    {
        private INavigationService _navigationService;
        
        public HomeViewModel(INavigationService navigationService) 
        {
            _navigationService = navigationService;
        }

        public ICommand NavigateToUploadCommand => new RelayCommand(() => NavigateTo(typeof(UploadViewModel)));
        public ICommand NavigateToGenerateCommand => new RelayCommand(() => NavigateTo(typeof(UploadViewModel)));
        public ICommand NavigateToValidateCommand => new RelayCommand(() => NavigateTo(typeof(ValidationViewModel)));

        public string Title => "Home Page";

        private void NavigateTo(Type targetViewModel)
        {
            if (targetViewModel != null)
            {
                _navigationService.NavigateTo(targetViewModel.FullName);
            }
        }
    }
}
