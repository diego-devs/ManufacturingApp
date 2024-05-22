using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ManufacturingApp.Models
{
    [Table("RecipeProduct")]
    public class RecipeProduct
    {
        [JsonPropertyName("recipeId")]
        public int RecipeId { get; set; }
        [JsonPropertyName("recipe")]
        public Recipe Recipe { get; set; }
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }
        [JsonPropertyName("product")]
        public Product Product { get; set; }
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }

    }
}
