using System.Text.Json.Serialization;

namespace ManufacturingApp.API.DTOs
{
    public class OptimizedSupplierDTO
    {
        [JsonPropertyName("supplierId")]
        public int SupplierId { get; set; }
        [JsonPropertyName("supplierName")]
        public string SupplierName { get; set; }
        [JsonPropertyName("rawMaterialPrices")]
        public Dictionary<int, decimal> RawMaterialPrices { get; set; } // Key: RawMaterialId, Value: Price
        [JsonPropertyName("totalCost")]
        public decimal TotalCost { get; set; }
    }
}
