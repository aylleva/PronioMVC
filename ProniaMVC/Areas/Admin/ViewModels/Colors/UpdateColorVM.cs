using System.ComponentModel.DataAnnotations;

namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class UpdateColorVM
    {
        [MaxLength(20, ErrorMessage = "Color must exist max 20 symbols!!")]
        public string Name { get; set; }
    }
}
