using System.Text;
using Validator.Application.Addresses;
using Validator.Domain.Addresses;
using Xunit;

namespace Application.Tests.Addresses
{
    public class CsvAddressParserTests
    {
        [Fact]
        public void ParseAddress_WithValidAddress_ReturnsAddress()
        {
            var csvContent = "ADRES;PC;WP;CONTACT;BEDRIJF\n" +
                             "Kerkstraat 1;1000;Brussel;John Doe;Microsoft\n" +
                             "Stationslaan 25;2000;Antwerpen;Jane Smith;Google";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var addresses = CsvAddressParser.ReadCsvFile(stream).ToList();

            var address1 = new AddressDetails
            {
                Street = "Kerkstraat 1",
                PostalCode = 1000,
                City = "Brussel",
                Contact = "John Doe",
                Company = "Microsoft"
            };

            var address2 = new AddressDetails
            {
                Street = "Stationslaan 25",
                PostalCode = 2000,
                City = "Antwerpen",
                Contact = "Jane Smith",
                Company = "Google"
            };


            // Assert
            Assert.Equal(address1, addresses[0].AddressDetails);
            Assert.Equal(address2, addresses[1].AddressDetails);
        }

        [Fact]
        public void ParseAddress_WithCompanyMissing_ReturnsAddress()
        {
            // Arrange
            var csvContent = "ADRES;PC;WP;CONTACT\n" +
                             "Kerkstraat 1;1000;Brussel;John Doe\n" +
                             "Stationslaan 25;2000;Antwerpen;Jane Smith";
            
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var addresses = CsvAddressParser.ReadCsvFile(stream).ToList();
            
            var address1 = new AddressDetails
            {
                Street = "Kerkstraat 1",
                PostalCode = 1000,
                City = "Brussel",
                Contact = "John Doe"
            };

            var address2 = new AddressDetails
            {
                Street = "Stationslaan 25",
                PostalCode = 2000,
                City = "Antwerpen",
                Contact = "Jane Smith"
            };


            // Assert
            Assert.Equal(address1.City, addresses[0].AddressDetails.City);
            Assert.Equal(address1.PostalCode, addresses[0].AddressDetails.PostalCode);
            Assert.Equal(address1.Street, addresses[0].AddressDetails.Street);
            Assert.Equal(address1.Contact, addresses[0].AddressDetails.Contact);
            
            Assert.Equal(address2.City, addresses[1].AddressDetails.City);
            Assert.Equal(address2.PostalCode, addresses[1].AddressDetails.PostalCode);
            Assert.Equal(address2.Street, addresses[1].AddressDetails.Street);
            Assert.Equal(address2.Contact, addresses[1].AddressDetails.Contact);
        }
    }
}
