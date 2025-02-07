using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Validator.Application.Mailings.Models;
using Validator.Domain.Addresses;
using Validator.Domain.MailingResponses.Models;
using Validator.Domain.Mailings.Models;

namespace WpfDesktop.Services
{
    public class ApplicationState
    {
        public bool HasUploadedAddressList { get; set; }
        public bool HasGeneratedMailingRequest { get; set; }
        public bool HasDownloadedMailingRequest { get; set; }
        public bool HasDownloadedMailingAddressList { get; set; }
        public bool HasValidatedAddresses { get; set; }
        public List<AddressLine> AddressList { get; set; } = [];

        public MailIdOptions MailIdOptions { get; private set; }

        public int AddressCount => AddressList.Count;

        public string OutputFormat { get; set; }

        public string SortingMode { get; set; }
        // Add any other state properties you need

        public MailIdRequest MailingRequest { get; set; }

        public MailingResponse MailingResponse { get; set; }

        public ValidationResponse ValidationResponse { get; set; }

        public ApplicationState()
        {
            MailIdOptions = InitializeMailIdOptions();
            OutputFormat = MailListFileOutputs.XML;
            SortingMode = "Customer Order (CU)";
        }

        private MailIdOptions InitializeMailIdOptions()
        {
            return new MailIdOptions 
            { 
                GenMid = false, 
                GenPSC = true,
                Mode = "T"
            };


        }
    }

}
