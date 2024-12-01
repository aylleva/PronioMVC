using ProniaMVC.Models.Base;

namespace ProniaMVC.Models;

public class ProductSizes
{
    public int id {  get; set; }
    public int ProductId {  get; set; }
    public int SizeId {  get; set; }
    public Product Product { get; set; }
    public Size Size { get; set; }
}
