using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ManufacturingApp.API.DTOs
{
    public class RawMaterialDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        [Required]
        public string Description { get; set; }
    }
}
