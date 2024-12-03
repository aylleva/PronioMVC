using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class GetProductVM
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
      
        public string CategoryName {  get; set; }
        public string Image {  get; set; }  
        public string SKU {  get; set; }
        
        public List<string> Colors { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Sizes { get; set; }

    }
}
