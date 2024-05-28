using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ManufacturingApp.API.DTOs
{
    public class SupplierDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("supplierRawMaterials")]
        public ICollection<SupplierRawMaterialDTO> SupplierRawMaterials { get; set; } = new List<SupplierRawMaterialDTO>();
    }
}
