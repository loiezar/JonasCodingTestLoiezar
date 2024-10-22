using System.Net;
using System.Web.Http;
using WebApi.Filters;

namespace WebApi.Controllers
{   

    [ResponseFilter]
    public abstract class BaseApiController : ApiController
    {
        
    }
}