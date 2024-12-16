namespace ProniaMVC.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId {  get; set; }
        public Product Product { get; set; }
        public string Userid {  get; set; }
        public AppUser User { get; set; }
        public int Count {  get; set; }
        public decimal Price {  get; set; }
        public int OrderId {  get; set; }
        public Order Order { get; set; }
    }
}
