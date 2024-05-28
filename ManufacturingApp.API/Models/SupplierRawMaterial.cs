using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManufacturingApp.Models
{
    [Table("SupplierRawMaterial")]
    public class SupplierRawMaterial
    {
        [JsonPropertyName("supplierId")]
        public int SupplierId { get; set; }

        [JsonPropertyName("supplier")]
        public Supplier Supplier { get; set; }

        [JsonPropertyName("rawMaterialId")]
        public int RawMaterialId { get; set; } 

        [JsonPropertyName("rawMaterial")]
        public RawMaterial RawMaterial { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
