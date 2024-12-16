namespace ProniaMVC.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {

                context.Response.Redirect($"/home/error?errormessage={e.Message}");
            }
        }
    }
}
