using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Services.Responses;

namespace Pharmacy.System.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult FromResponse(Response response)
            => StatusCode(response.StatusCode, response);
        
        protected IActionResult FromResponse<T>(Response<T> response)
            => StatusCode(response.StatusCode, response);
    }
}
