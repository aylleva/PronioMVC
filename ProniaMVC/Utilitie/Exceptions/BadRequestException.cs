namespace ProniaMVC.Utilitie.Exceptions
{
    public class BadRequestException:Exception
    {
        public BadRequestException(string message):base(message) { }

        public BadRequestException():base("Wrong Request!") { }
        
    }
}
