using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.Http.Filters;

namespace WebApi.Filters
{
    public class ResponseFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Exception == null)
            {
                //i am not sure if you want a structured response or just receive the raw json data.

                //var response = actionExecutedContext.Response;

                //if (response != null)
                //{
                //    if (response.IsSuccessStatusCode && response.Content is ObjectContent)
                //    {
                //        var originalContent = await response.Content.ReadAsAsync<object>(cancellationToken);

                //        var wrappedResponse = new
                //        {
                //            Data = originalContent,
                //            Status = "Success",
                //            Data = originalContent,
                //            Timestamp = System.DateTime.UtcNow
                //        };

                //        actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, wrappedResponse);
                //    }
                //}
            }
            else
            {
                var errorResponse = new
                {
                    Message = "An Error has occured.",
                    Details = actionExecutedContext.Exception.Message

                };

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, errorResponse);
            }

            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}