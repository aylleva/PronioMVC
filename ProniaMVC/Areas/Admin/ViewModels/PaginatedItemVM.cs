namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class PaginatedItemVM<T>
    {
        public double TotalPage {  get; set; }
        public int CurrectPage {  get; set; }
        public List<T> Items { get; set; }
    }
}
