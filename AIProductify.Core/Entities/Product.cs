

using System.ComponentModel.DataAnnotations.Schema;
using AIProductify.Core.Common;

namespace AIProductify.Core.Entities
{
    public class Product : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Sku { get; set; }
        public List<string>? ParentSku { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        [NotMapped]
        public List<(string Sku, string Name)> ParentSkuWithDetails { get; set; }
        public List<ProductAttribute>? Attributes { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public List<string>? Images { get; set; }

    }

    
}
