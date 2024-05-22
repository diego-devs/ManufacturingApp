using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingApp.Models
{
    [Table("RecipeRawMaterial")]
    public class RecipeRawMaterial
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int RawMaterialId { get; set; }
        public RawMaterial RawMaterial {get;set;} 
        public decimal Quantity { get; set; }
    }
}