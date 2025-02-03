using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Services
{
    public class BarcodeGenerator
    {
        private readonly string _customerBarcodeId; // 5 digits, provided by bpost
        private int _index = 0;
        private const string _formatControlField = "12";
        private string _sequenceNumber;
        private string _dayOfTheYear;

        public BarcodeGenerator(int customerBarcodeId, int sequenceNumber, int dayOfTheYear  )
        {
            _customerBarcodeId = customerBarcodeId.ToString().PadLeft(5, '0');
            _sequenceNumber = sequenceNumber.ToString().PadLeft(2, '0');
            _dayOfTheYear = dayOfTheYear.ToString().PadLeft(3, '0');
        }

        //This number consists of three(consecutive) fields in the barcode: 
        // The format control code field(B) (2)
        // Customer identification(C) (5)
        // Mail piece number(D) (11)
        //The MAIL ID number has to be unique over a period of 30 days at least 

        //Mailingman splits up the Mail piece number in three parts:
        // A variable assigned by user choice (2)
        // The day of the year (3)
        // A sequence number (7)

        public string GenerateNext()
        {
            _index++;

            return $"{_formatControlField}{_customerBarcodeId}{_sequenceNumber}{_dayOfTheYear}{_index.ToString().PadLeft(6, '0')}";
        }       
    }
}
