using ProniaMVC.Models;

namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class AdminOrdersVM
    {
        public int OrderId {  get; set; }
        public string UserName {  get; set; }

        public decimal Subtotal { get; set; }
        public bool? Status { get; set; }
        public string CreatedAt { get; set; }
    }
}
