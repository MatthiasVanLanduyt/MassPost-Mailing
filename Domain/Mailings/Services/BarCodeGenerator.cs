using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Mailings.Services
{
    public class BarcodeGenerator
    {
        private readonly string _customerBarcodeId; // 5 digits, provided by bpost
        private int _currentSequence = 0;

        public BarcodeGenerator(string customerBarcodeId)
        {
            _customerBarcodeId = customerBarcodeId.PadLeft(5, '0');
        }

        public string GenerateNext()
        {
            _currentSequence++;
            // Format: FCC (10) + CustomerBarcodeID (5) + Sequence (7)
            return $"10{_customerBarcodeId}{_currentSequence.ToString().PadLeft(7, '0')}";
        }
    }
}
