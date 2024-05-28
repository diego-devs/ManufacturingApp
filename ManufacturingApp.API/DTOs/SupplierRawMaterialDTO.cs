using System.Text.Json.Serialization;

namespace ManufacturingApp.API.DTOs
{
    public class SupplierRawMaterialDTO
    {
        [JsonPropertyName("rawMaterialId")]
        public int RawMaterialId { get; set; }

        [JsonPropertyName("rawMaterialName")]
        public string RawMaterialName { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

    }
}
