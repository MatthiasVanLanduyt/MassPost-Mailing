using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Domain.Addresses;

namespace WpfDesktop.Services
{
    public class ApplicationState
    {
        public bool HasUploadedAddressList { get; set; }
        public bool HasGeneratedMailingFile { get; set; }
        public bool HasValidatedAddresses { get; set; }
        public List<AddressLine> AddressList { get; set; } = [];

        public int AddressCount => AddressList.Count;
        // Add any other state properties you need
    }

}
