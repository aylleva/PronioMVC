using ProniaMVC.Models.Base;

namespace ProniaMVC.Models
{
    public class Order:BaseEntity
    {
        public string Address {  get; set; }
        public int Id { get; set; }
        public string Userid { get; set; }
        public AppUser User { get; set; }

        public List<OrderItem> Orderitems { get; set; }
        public decimal Subtotal {  get; set; }
        public bool? Status { get; set; }



    }
}
