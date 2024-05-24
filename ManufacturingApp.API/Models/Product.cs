using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManufacturingApp.Models
{
    [Table("Product")]
    public class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        [Required]
        public string Description { get; set; }
        [JsonPropertyName("sellingPrice")]
        [Required]
        public decimal SellingPrice { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public ICollection<RecipeProduct> RecipeProducts { get; set; } = new List<RecipeProduct>();
    }
}
