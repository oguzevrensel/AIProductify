using System.Text.Json.Serialization;
using AIProductify.Application.DTO.Attribute;

namespace AIProductify.Application.DTO.Product
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Sku { get; set; }
        public List<string>? ParentSku { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public List<ProductAttributeDto> Attributes { get; set; }

        [JsonIgnore] 
        public List<(string Sku, string Name)> ParentSkuWithDetails { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public List<string> Images { get; set; }
    }
}
