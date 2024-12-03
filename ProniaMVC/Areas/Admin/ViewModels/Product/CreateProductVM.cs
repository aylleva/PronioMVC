using ProniaMVC.Models;

using ProniaMVC.Models.Base;
using System.ComponentModel.DataAnnotations;

using Size = ProniaMVC.Models.Base.Size;


namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class CreateProductVM
    {
        public IFormFile MainPhoto { get; set; }
        public IFormFile HoverPhoto { get; set; }

        public List<IFormFile> AdditionalPhotos { get; set; }
        public string Name { get; set; }

        [Required]
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }

        [Required]
        public int? CategoryId { get; set; }
      
       
        public List<Category>? Categories { get; set; }

        public List<Tag>? Tags { get; set; }
        public List<int>? Tagids { get; set; }

        public List<Color>? Colors { get; set; }
        public List<int>? ColorIds { get; set; }

        public List<Size>? Sizes { get; set; }
        public List<int>? SizeIds { get; set; }

    }
}
