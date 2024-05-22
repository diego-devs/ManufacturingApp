using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManufacturingApp.Models
{
    [Table("RecipeRawMaterial")]
    public class RecipeRawMaterial
    {
        [JsonPropertyName("recipeId")]
        public int RecipeId { get; set; }
        [JsonPropertyName("recipe")]
        public Recipe Recipe { get; set; }
        [JsonPropertyName("rawMaterialId")]
        public int RawMaterialId { get; set; }
        [JsonPropertyName("rawMaterial")]
        public RawMaterial RawMaterial {get;set;}
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
    }
}