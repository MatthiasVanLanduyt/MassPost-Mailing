using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.TypeConversion;
using Validator.Domain.Addresses;

namespace Validator.Application.Addresses
{
    public static class CsvAddressParser
    {
        public static IEnumerable<AddressLine> ReadCsvFile(Stream fileStream)
        {
            List<AddressLine> addresses = new List<AddressLine>();
            try
            {
                var culture = new CultureInfo("nl-BE");
                Console.WriteLine("Reading CSV file");
                using (var reader = new StreamReader(fileStream))
                using (var csv = new CsvReader(reader, culture))
                {
                    csv.Context.RegisterClassMap<AddressMap>();

                    csv.Read();
                    csv.ReadHeader();

                    Debug.WriteLine($"Header length: {csv.HeaderRecord.Length}");
                    Debug.WriteLine($"Current index: {csv.CurrentIndex}");

                    while (csv.Read())
                    {
                        var record = csv.GetRecord<AddressLine>();
                        Debug.WriteLine($"Record: {record}");
                        addresses.Add(record);
                        // Do something with the record.
                    }
                }
            }
            catch (HeaderValidationException ex)
            {
                // Specific exception for header issues
                throw new ApplicationException("CSV file header is invalid.", ex);
            }
            catch (TypeConverterException ex)
            {
                // Specific exception for type conversion issues
                throw new ApplicationException("CSV file contains invalid data format.", ex);
            }
            catch (Exception ex)
            {
                // General exception for other issues
                throw new ApplicationException("Error reading CSV file", ex);
            }
            return addresses;
        }
    }
}
