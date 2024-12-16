namespace ProniaMVC.Utilitie.Exceptions
{
    public class NotFountException : Exception
    {
        public NotFountException(string message) : base(message) { }
        public NotFountException():base("Not Found!") { }
      
    }
}
