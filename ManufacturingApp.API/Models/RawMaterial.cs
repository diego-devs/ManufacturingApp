using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingApp.Models
{
    [Table("RawMaterial")]
    public class RawMaterial
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<SupplierRawMaterial> SupplierRawMaterials {get;set;}
        public ICollection<RecipeRawMaterial> RecipeRawMaterials { get;set;}

    }
}
