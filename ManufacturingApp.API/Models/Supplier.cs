using Azure.Core.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManufacturingApp.Models
{
    [Table("Supplier")]
    public class Supplier
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<SupplierRawMaterial> SupplierRawMaterials { get; set; } = new List<SupplierRawMaterial>();
    }
}
