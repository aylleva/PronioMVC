using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.ViewModels.Product
{
    public class DetailProductVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
      
        public string SKU { get; set; }

        public string CategoryName {  get; set; }
        public List<ProductTags> Tags { get; set; }
        public List<ProductColors> Colors { get; set; }
        public List<ProductSizes> Sizes { get; set; }
      
        public string Image {  get; set; }
    }
}
