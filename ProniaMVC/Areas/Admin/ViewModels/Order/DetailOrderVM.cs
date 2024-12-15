using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class DetailOrderVM
    {
        public string Image {  get; set; }
        public string Name {  get; set; }
        public decimal Price { get; set; }
        public string Address {  get; set; }
        public int Count {  get; set; }
       
    }
}
