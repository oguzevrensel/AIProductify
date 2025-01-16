

namespace AIProductify.Core.Entities
{
    public class ProductAttribute
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        // Foreign Key
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
