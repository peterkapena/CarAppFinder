using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarAppFinder.Services.Bug
{

    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public HttpResponseExceptionFilter(IErrorLogService errorLogService)
        {
            ErrorLogService = errorLogService;
        }
        public int Order { get; } = int.MaxValue - 10;
        public IErrorLogService ErrorLogService { get; }

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                Task.Run(() => ErrorLogService.RegisterError(context.Exception));

                context.Result =
                    new ObjectResult(new Dictionary<string, string> {
                    {
                            "error",
                            "An error happened. Please contact support"
                    },
                    {
                            "message",
                            context.Exception.Message
                    } } )

                    {
                        StatusCode = 500
                    };
                context.ExceptionHandled = true;
            }
        }
    }
}
