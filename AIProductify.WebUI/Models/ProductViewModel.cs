using Microsoft.AspNetCore.Mvc;

namespace AIProductify.WebUI.Models
{
    public class ProductViewModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Sku { get; set; }
        public List<string>? ParentSku { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public List<ProductAttributeViewModel>? Attributes { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public List<string>? Images { get; set; }

        public decimal Score { get; set; }

    }
}
