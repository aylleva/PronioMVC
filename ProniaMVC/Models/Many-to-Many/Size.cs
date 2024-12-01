
using System.ComponentModel.DataAnnotations;

namespace ProniaMVC.Models.Base;

public class Size:BaseEntity
{
   
    public string Name {  get; set; }
    public List<ProductSizes> ProductSizes { get; set; }
}
