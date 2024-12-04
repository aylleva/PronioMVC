using System.ComponentModel.DataAnnotations;

namespace ProniaMVC.ViewModels
{
    public class LoginVM
    {
        [MinLength(5)]
        [MaxLength(256)]
        public string UserorEmail {  get; set; }

        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }    

        public bool IsPersisted {  get; set; }
    }
}
