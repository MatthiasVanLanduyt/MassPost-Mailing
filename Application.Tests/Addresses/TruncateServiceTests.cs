using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator.Application.Mailings.Models;
using Validator.Application.Mailings.Services;
using Validator.Domain.Mailings.Models;
using Xunit;

namespace Application.Tests.Addresses
{
    namespace Validator.Tests.Application.Mailings
    {
        public class TruncateFieldsToMaxLengthServiceTests
        {
            private readonly TruncateFieldsToMaxLengthService _service;

            public TruncateFieldsToMaxLengthServiceTests()
            {
                _service = new TruncateFieldsToMaxLengthService();
            }

            [Fact]
            public void TruncateAddressComponent_WithNullValue_ReturnsNull()
            {
                // Arrange
                var component = new AddressComponent
                {
                    Code = 2,
                    Value = null
                };

                // Act
                var result = _service.TruncateAddressComponent(component);

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void TruncateAddressComponent_WithEmptyValue_ReturnsEmptyString()
            {
                // Arrange
                var component = new AddressComponent
                {
                    Code = 2,
                    Value = string.Empty
                };

                // Act
                var result = _service.TruncateAddressComponent(component);

                // Assert
                Assert.Equal(string.Empty, result);
            }

            [Theory]
            [InlineData(2, "John", "John")] // FirstName under max length
            [InlineData(2, "ThisIsAVeryLongFirstNameThatExceedsTheMaximumLengthAllowedForFirstNames", "ThisIsAVeryLongLastNameThatExceedsTheMaxi")] // FirstName truncated
            [InlineData(4, "Smith", "Smith")] // LastName under max length
            [InlineData(4, "ThisIsAVeryLongLastNameThatExceedsTheMaximumLengthAllowedForLastNames", "ThisIsAVeryLongLastNameThatExceedsTheMaxim")] // LastName truncated
            [InlineData(16, "Brussels", "Brussels")] // City under max length
            [InlineData(16, "ThisIsAReallyLongCityNameThatShouldBeTruncated", "ThisIsAReallyLongCityNameThatS")] // City truncated
            [InlineData(92, "123 Main Street", "123 Main Street")] // Street under max length
            [InlineData(92, "ThisIsAReallyLongStreetNameThatExceedsTheMaximumLengthAllowedForStreetNames", "ThisIsAReallyLongStreetNameThatExceedsTheMaximumLe")] // Street truncated
            public void TruncateAddressComponent_WithDifferentCodes_TruncatesCorrectly(int code, string input, string expected)
            {
                // Arrange
                var component = new AddressComponent
                {
                    Code = code,
                    Value = input
                };

                // Act
                var result = _service.TruncateAddressComponent(component);

                // Assert
                Assert.Equal(expected, result);
                Assert.True(result.Length <= MaxFieldLengths.GetMaxLength(code));
            }

            [Fact]
            public void TruncateAddressComponent_WithUnknownCode_UseDefaultMaxLength()
            {
                // Arrange - using code 75 which should use the default max length
                var longValue = new string('A', 100);
                var component = new AddressComponent
                {
                    Code = 75,
                    Value = longValue
                };

                // Act
                var result = _service.TruncateAddressComponent(component);

                // Assert
                Assert.Equal(longValue.Substring(0, MaxFieldLengths.Custom), result);
                Assert.Equal(MaxFieldLengths.Custom, result.Length);
            }

            [Fact]
            public void TruncateAddressComponent_WithBorderlineLength_DoesNotTruncate()
            {
                // Arrange - create a string exactly the max length for FirstName
                int maxLength = MaxFieldLengths.FirstName;
                var borderlineValue = new string('A', maxLength);
                var component = new AddressComponent
                {
                    Code = 2, // FirstName
                    Value = borderlineValue
                };

                // Act
                var result = _service.TruncateAddressComponent(component);

                // Assert
                Assert.Equal(borderlineValue, result);
                Assert.Equal(maxLength, result.Length);
            }

            [Theory]
            [InlineData(1, 10)]  // Greeting
            [InlineData(2, 42)]  // FirstName
            [InlineData(3, 20)]  // MiddleName
            [InlineData(4, 42)]  // LastName
            [InlineData(5, 10)]  // Suffix
            [InlineData(6, 42)]  // CompanyName
            [InlineData(7, 42)]  // Department
            [InlineData(8, 42)]  // Building
            [InlineData(9, 42)]  // StreetLine
            [InlineData(12, 12)] // HouseNumber
            [InlineData(13, 8)]  // BoxNumber
            [InlineData(14, 42)] // POBoxNumber
            [InlineData(15, 12)] // PostalCode
            [InlineData(16, 30)] // City
            [InlineData(17, 2)]  // CountryCode
            [InlineData(18, 42)] // CountryName
            [InlineData(19, 42)] // State
            [InlineData(90, 50)] // UnstructuredName
            [InlineData(91, 50)] // UnstructuredCompany
            [InlineData(92, 50)] // UnstructuredStreet
            [InlineData(93, 50)] // UnstructuredPostCodeCity
            public void TruncateAddressComponent_WithAllComponentCodes_UsesCorrectMaxLength(int code, int expectedMaxLength)
            {
                // Arrange - create a string that's definitely longer than any max length
                var longValue = new string('A', 100);
                var component = new AddressComponent
                {
                    Code = code,
                    Value = longValue
                };

                // Act
                var result = _service.TruncateAddressComponent(component);

                // Assert
                Assert.Equal(expectedMaxLength, result.Length);
                Assert.Equal(longValue.Substring(0, expectedMaxLength), result);
            }
        }
    }
}
