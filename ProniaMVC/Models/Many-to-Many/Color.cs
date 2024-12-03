
using System.ComponentModel.DataAnnotations;

namespace ProniaMVC.Models.Base;

public class Color : BaseEntity
{
  
    public string Name { get; set; }
    public List<ProductColors> ProductColors { get; set; }

}


