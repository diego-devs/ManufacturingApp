using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ManufacturingApp.API.DTOs
{
    public class RecipeDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        [Required]
        public string Description { get; set; }

        [JsonPropertyName("recipeRawMaterials")]
        public ICollection<RecipeRawMaterialDTO> RecipeRawMaterials { get; set; } = new List<RecipeRawMaterialDTO>();

        [JsonPropertyName("recipeProducts")]
        public ICollection<RecipeProductDTO> RecipeProducts { get; set; } = new List<RecipeProductDTO>();

        [JsonPropertyName("recipeSuppliers")]
        public List<RecipeSupplierDTO> RecipeSuppliers { get; set; } = new List<RecipeSupplierDTO>();
    }
}
