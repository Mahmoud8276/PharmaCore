using Asp.Versioning;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [Authorize(Roles = "Admin")]
    [ApiVersion(1.0)]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        // GET api/v1/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserSpecParams specParams)
        {
            return FromResponse(
                await _userService.GetAllAsync(specParams));
        }

        // GET api/v1/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            return FromResponse(
                await _userService.GetByIdAsync(id));
        }
    }
}
