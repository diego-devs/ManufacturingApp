using System.Text.Json.Serialization;

namespace ManufacturingApp.API.DTOs
{
    public class RecipeSupplierDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pricing")]
        public Dictionary<string, decimal> Pricing { get; set; } = new Dictionary<string, decimal>();// Raw material name to price
    }
}