using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Domain.Addresses
{
    public static class AddressErrors
    {
        public static string PostalCodeIsEmpty =>"Postal code must not be empty";

        public static string PostalCodeIsIncorrectLength => "Postal code must be 4 characters long";

        public static string PostalCodeDoesNotExist => "Postal code does not exist";

        public static string InvalidPostalCode => "Invalid postal code";

        public static string InvalidStreet => "Invalid street";

        public static string InvalidCity => "Invalid city";

        public static string PostalCodeCityMismatch => "Incorrect postal code/city combination";

    }
}