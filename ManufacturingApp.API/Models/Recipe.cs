using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingApp.Models
{
    [Table("Recipe")]
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<RecipeRawMaterial> RecipeRawMaterials { get; set; }
        public ICollection<RecipeProduct> RecipeProducts { get; set; }
        
    }
}
