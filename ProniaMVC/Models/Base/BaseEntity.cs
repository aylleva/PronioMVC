namespace ProniaMVC.Models.Base
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateTime { get; set; }
    }
}
