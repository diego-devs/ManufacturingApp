using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManufacturingApp.Models
{
    [Table("Recipe")]
    public class Recipe
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("recipeRawMaterials")]
        public ICollection<RecipeRawMaterial> RecipeRawMaterials { get; set; } = new List<RecipeRawMaterial>();
        [JsonPropertyName("recipeProducts")]
        public ICollection<RecipeProduct> RecipeProducts { get; set; } = new List<RecipeProduct>();
        
    }
}
