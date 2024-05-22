using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManufacturingApp.Models
{
    [Table("SupplierRawMaterial")]
    public class SupplierRawMaterial
    {
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public int RawMaterialId { get; set; }
        public RawMaterial RawMaterial { get; set; }
        public decimal Price { get; set; }
    }
}
