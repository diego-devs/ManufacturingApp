using System.Text.Json.Serialization;

namespace ManufacturingApp.API.DTOs
{
    public class RecipeProductDTO
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("productName")]
        public string ProductName { get; set; }

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
    }
}
