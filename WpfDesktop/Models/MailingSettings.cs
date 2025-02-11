using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Validator.Domain.Mailings.Models;

namespace WpfDesktop.Models
{
    public class MailingSettings
    {

        public string OutputFormat { get; set; } = MailListFileOutputs.XML;
        public string DefaultSaveLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string Mode { get; set; } = "P";
        public bool GenerateMailIds { get; set; } = false;
        public bool GeneratePreSorting { get; set; } = true;
        public string SortingMode { get; set; } = "Customer Order (CU)";
       
        public string SenderId { get; set; } = "4493";

        public string AddressLanguage { get; set; } = "nl";

        public string Priority { get; set; } = "NP";
    }
}
