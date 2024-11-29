using ProniaMVC.Models.Base;

namespace ProniaMVC.Models
{
    public class Tag:BaseEntity
    {
      public string Name {  get; set; }
      public List<ProductTags> Tags { get; set; }
    }
}
