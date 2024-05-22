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
        [JsonPropertyName("supplierRawMaterials")]
        public ICollection<SupplierRawMaterial> SupplierRawMaterials {get;set;}
        [JsonPropertyName("recipeRawMaterials")]
        public ICollection<RecipeRawMaterial> RecipeRawMaterials { get;set;}

    }
}
