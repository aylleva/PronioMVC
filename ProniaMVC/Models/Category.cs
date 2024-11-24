using ProniaMVC.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace ProniaMVC.Models
{
    public class Category:BaseEntity
    {
        [MaxLength(20,ErrorMessage ="You must add max 20 symbols")]
        public string Name {  get; set; }
        public List<Product>? Product { get; set; }
    }
}
