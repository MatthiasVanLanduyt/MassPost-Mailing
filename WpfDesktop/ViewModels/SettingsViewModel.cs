using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Validator.Domain.Mailings.Models;
using WpfDesktop.Services;

namespace WpfDesktop.ViewModels
{
    public partial class SettingsViewModel: ObservableObject
    {
        
        [ObservableProperty]
        private string outputFormat;

        [ObservableProperty]
        private string mode;

        [ObservableProperty]
        private bool generateMailIds;

        [ObservableProperty]
        private bool generatePreSorting;

        [ObservableProperty]
        private string sortingMode;

        public List<string> OutputFormatOptions { get; } = new() { MailListFileOutputs.XML, MailListFileOutputs.TXT };
        public List<string> ModeOptions { get; } = new() { "T", "P", "C" }; // Test, Production, Certification
        public List<string> SortingModeOptions { get; } = new() { "Customer Order (CU)", "Print Order (PO)" };

        public ICommand SaveCommand => _saveCommand;

        private readonly ICommand _saveCommand;
        
        
        
        //Services
        private readonly ApplicationState _state;




        public SettingsViewModel(ApplicationState state)
        {
            _state = state;

            // Initialize from global state
            outputFormat = _state.OutputFormat;
            mode = _state.MailIdOptions.Mode;
            generateMailIds = _state.MailIdOptions.GenMid;
            generatePreSorting = _state.MailIdOptions.GenPSC;
            sortingMode = _state.SortingMode;
        }

        
    }
}
