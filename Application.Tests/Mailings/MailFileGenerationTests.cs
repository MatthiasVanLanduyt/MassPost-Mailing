using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharedkernel;
using System.Xml.Linq;
using Validator.Application.Mailings.Models;
using Validator.Application.Mailings.Services;
using Validator.Domain.Addresses;
using Validator.Domain.Mailings.Models;
using Xunit;
using Validator.Application.Mailings.Contracts;

namespace Application.Tests.Mailings
{
    public class MailIdFileGeneratorTests
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IMailIdFileGenerator _xmlGenerator;

        public MailIdFileGeneratorTests()
        {
            _dateTimeProvider = new MockDateTimeProvider();
            _xmlGenerator = new XmlMailIdFileGenerator(_dateTimeProvider);
        }

        [Fact]
    public void XmlGenerator_WithOversizedComponents_TruncatesValuesInOutput()
    {
        // Arrange
        var request = CreateMailIdRequestWithOversizedComponents();

        // Act
        var file = _xmlGenerator.GenerateFile(request);
        var xmlContent = System.Text.Encoding.UTF8.GetString(file.Content);
        var xml = XDocument.Parse(xmlContent);

        // Assert
        var components = xml.Descendants("Comp").ToList();
        foreach (var component in components)
        {
            int code = int.Parse(component.Attribute("code")!.Value);
            string value = component.Attribute("value")!.Value;
            int maxLength = MaxFieldLengths.GetMaxLength(code);

            Assert.True(value.Length <= maxLength,
                $"Component with code {code} has length {value.Length}, which exceeds the max length {maxLength}");
        }
    }

    private MailIdRequest CreateMailIdRequestWithOversizedComponents()
    {
        // Create components with values longer than allowed
        var oversizedComponents = new[]
        {
                new AddressComponent { Code = 2, Value = new string('A', 100) }, // FirstName (max 42)
                new AddressComponent { Code = 4, Value = new string('B', 100) }, // LastName (max 42)
                new AddressComponent { Code = 9, Value = new string('C', 100) }, // StreetLine (max 42)
                new AddressComponent { Code = 16, Value = new string('D', 100) }, // City (max 30)
                new AddressComponent { Code = 92, Value = new string('E', 100) }  // UnstructuredStreet (max 50)
            };

        // Create a mail item with these components
        var mailItem = new MailIdItem
        {
            Sequence = 1,
            MailIdNum = "12345678901",
            Priority = "NP",
            Language = "nl",
            Components = oversizedComponents.ToList(),
            AddressDetails = new AddressDetails
            {
                Contact = "Test Contact",
                Street = "Test Street",
                City = "Test City",
                PostalCode = 1000
            }
        };

        // Create a request with this item
        return new MailIdRequest
        {
            Header = new MailIdRequestHeader
            {
                SenderId = 12345,
                AccountId = 67890,
                MailingRef = "TEST-REF",
                ExpectedDeliveryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
                CustomerBarcodeId = 54321,
                CustomerFileRef = "TEST-FILE-REF",
                DepositType = DepositIdentifierTypes.DepositReference,
                DepositIdentifier = "TEST-DEPOSIT"
            },
            Options = new MailIdOptions
            {
                GenMid = false,
                GenPsc = false,
                Mode = "T"
            },
            MailFormat = MailFormats.SmallFormat,
            MailFileInfo = MailingTypes.MailId,
            Contacts = new List<Contact>(),
            Items = new List<MailIdItem> { mailItem }
        };
    }

    private class MockDateTimeProvider : IDateTimeProvider
    {
            public string DateStamp => "20250101";
            public string TimeStamp => "20250101120000";
            public int DayOfTheYear => 1;

            public DateTime UtcNow => DateTime.UtcNow;

            public DateOnly DateNow => DateOnly.FromDateTime(UtcNow);
        }
}
}

