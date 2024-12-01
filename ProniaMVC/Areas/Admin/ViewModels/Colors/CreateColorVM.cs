using System.ComponentModel.DataAnnotations;

namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class CreateColorVM
    {
        [MaxLength(20, ErrorMessage = "Color must exist max 20 symbols!!")]
        public string Name { get; set; }    
    }
}
