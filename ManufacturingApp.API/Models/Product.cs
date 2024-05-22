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
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("sellingPrice")]
        public decimal SellingPrice { get; set; }
        [JsonPropertyName("recipeProducts")]
        public ICollection<RecipeProduct> RecipeProducts { get; set; }
    }
}
