using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfDesktop.ViewModels
{
    public class UploadViewModel: ObservableObject
    {
        public UploadViewModel() { }
        public string Title => "Upload Page";
    }
}
