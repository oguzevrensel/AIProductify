using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProductify.Core.Entities
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Sku { get; set; }
        public List<string> ParentSku { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public List<Attribute> Attributes { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public List<string> Images { get; set; }

    }

    public class Attribute
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }
}
