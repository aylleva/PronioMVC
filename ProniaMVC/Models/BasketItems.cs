namespace ProniaMVC.Models
{
    public class BasketItems
    {
        public int Id { get; set; }

        public int ProductId {  get; set; }
        public Product Product { get; set; }

        public string Userid {  get; set; }
        public AppUser User { get; set; }
        public int Count {  get; set; }
    }
}
