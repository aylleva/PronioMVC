using ProniaMVC.Models.Base;
namespace ProniaMVC.Models
{
    public class ProductColors
    {
        public int id {  get; set; }
        public int ColorId {  get; set; }
        public int ProductId {  get; set; }
        public Color Color { get; set; }    
        public Product Product { get; set; }
    }
}
