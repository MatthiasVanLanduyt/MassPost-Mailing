using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validator.Application.Mailings.Models
{
    public static class MaxFieldLengths
    {
        public const int MailingRef = 20;
        public const int DepositIdentifier = 20;
        public const int DepositIdentifierType = 20;
        public const int CustomerFileRef = 10;
        public const int AccountId = 8;
        public const int CustomerId = 8;
        public const int ExpectedDeliveryDate = 10;
        public const int Sender = 8;
        public const int Version = 4;

        // Contact group
        public const int ContactSequence = 8; 
        public const int ContactFirstName = 50;
        public const int ContactLastName = 50;
        public const int ContactEmail = 100;
        public const int ContactPhone = 50;
        public const int ContactMobile = 50;
        public const int ContactLanguage = 2;

        //ItemGroup 
        public const int ItemSequence = 8;
        public const int ItemCount = 8;

        public const int ItemLanguage = 2;
        public const int ItemMailIdNum = 18;
        public const int ItemPsCode = 20;

        public const int ItemPriority = 2;

        public const int ItemAddressComponentKey = 2;
        public const int ItemAddressComponentValue = 70;

        public const int Greeting = 10;
        public const int FirstName = 42;
        public const int MiddleName = 20;
        public const int LastName = 42;
        public const int Suffix = 10;

        // Organisation and geolocation group
        public const int CompanyName = 42;
        public const int Department = 42;
        public const int Building = 42;

        // Street, house number and box number group
        public const int StreetLine = 42;
        public const int HouseNumber = 12;
        public const int BoxNumber = 8;
        public const int POBoxNumber = 42;

        // Postal code and city group
        public const int PostalCode = 12;
        public const int City = 30;

        // Country group
        public const int CountryCode = 2;
        public const int CountryName = 42;
        public const int State = 42;

        // Unstructured fields
        public const int UnstructuredName = 50;
        public const int UnstructuredCompany = 50;
        public const int UnstructuredStreet = 50;
        public const int UnstructuredPostcodeCity = 50;

        // Custom fields
        public const int Custom = 70;

        // Helper method to get max length by component code
        public static int GetMaxLength(int code)
        {
            return code switch
            {
                1 => Greeting,
                2 => FirstName,
                3 => MiddleName,
                4 => LastName,
                5 => Suffix,
                6 => CompanyName,
                7 => Department,
                8 => Building,
                9 => StreetLine,
                12 => HouseNumber,
                13 => BoxNumber,
                14 => POBoxNumber,
                15 => PostalCode,
                16 => City,
                17 => CountryCode,
                18 => CountryName,
                19 => State,
                90 => UnstructuredName,
                91 => UnstructuredCompany,
                92 => UnstructuredStreet,
                93 => UnstructuredPostcodeCity,
                _ => Custom // Default max length for other codes (70-79)
            };
        }
    }
}
