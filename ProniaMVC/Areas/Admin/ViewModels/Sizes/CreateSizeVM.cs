using System.ComponentModel.DataAnnotations;

namespace ProniaMVC.Areas.Admin.ViewModels.Sizes
{
    public class CreateSizeVM
    {
        [MaxLength(20, ErrorMessage = "Size must exist max 20 symbols!!")]
        public string Name {  get; set; }
    }
}
