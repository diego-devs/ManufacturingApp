using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingApp.Models
{
    [Table("Supplier")]
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<SupplierRawMaterial> SupplierRawMaterials { get; set; }
    }
}
