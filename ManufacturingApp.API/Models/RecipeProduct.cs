using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingApp.Models
{
    [Table("RecipeProduct")]
    public class RecipeProduct
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Quantity { get; set; }

    }
}
