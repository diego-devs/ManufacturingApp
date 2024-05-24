using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManufacturingApp.Models
{
    [Table("RawMaterial")]
    public class RawMaterial
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<SupplierRawMaterial> SupplierRawMaterials {get;set;} = new List<SupplierRawMaterial>();
        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<RecipeRawMaterial> RecipeRawMaterials { get; set; } = new List<RecipeRawMaterial>();

    }
}
