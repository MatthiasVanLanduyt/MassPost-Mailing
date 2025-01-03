using System.Text.Json;
using Validator.Domain.Addresses;

namespace Validator.Application.Addresses
{
    internal class PostalCodeService : IPostalCodeService
    {
        public Dictionary<string, HashSet<string>> PostalCodeCities { get; }

        public PostalCodeService(string filePath)
        {
            PostalCodeCities = LoadPostalCodesFromFile(filePath);
        }

        private static Dictionary<string, HashSet<string>> LoadPostalCodesFromFile(string filePath)
        {
            try
            {
                var jsonText = File.ReadAllText(filePath);

                // Since the JSON is an array of objects, we deserialize to a list
                var locations = JsonSerializer.Deserialize<List<BelgianPostalCodeCity>>(jsonText)
                    ?? throw new JsonException("Failed to deserialize postal code data");

                // Group by postal code and collect all valid city names (both Plaatsnaam and Hoofdgemeente)
                return locations
                    .GroupBy(x => x.PostalCode.ToString())
                    .ToDictionary(
                        group => group.Key,
                        group => new HashSet<string>(
                            group.SelectMany(location => new[]
                            {
                            location.City.ToUpperInvariant(),
                            }).Distinct()
                        )
                    );
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException)
            {
                throw new InvalidOperationException($"Error loading postal codes from {filePath}: {ex.Message}", ex);
            }
        }
    }
}
