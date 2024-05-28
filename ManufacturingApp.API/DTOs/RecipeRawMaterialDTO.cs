using System.Text.Json.Serialization;

namespace ManufacturingApp.API.DTOs
{
    public class RecipeRawMaterialDTO
    {
        [JsonPropertyName("rawMaterialId")]
        public int RawMaterialId { get; set; }
        [JsonPropertyName("rawMaterialName")]
        public string RawMaterialName { get; set; }

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
    }
}
