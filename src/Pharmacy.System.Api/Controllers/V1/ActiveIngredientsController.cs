using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.ActiveIngredientDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class ActiveIngredientsController : BaseController
    {
        private readonly IActiveIngredientService _activeIngredientService;
        public ActiveIngredientsController(IActiveIngredientService activeIngredientService)
        {
            _activeIngredientService = activeIngredientService;
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ActiveIngredientDto activeIngredientDto)
        {
            return FromResponse(
                await _activeIngredientService.CreateAsync(activeIngredientDto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] ActiveIngredientSpecParams specParams)
        {
            return FromResponse(
                await _activeIngredientService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(
                await _activeIngredientService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(
            int id,
            [FromBody] ActiveIngredientDto activeIngredientDto)
        {
            return FromResponse(
                await _activeIngredientService.UpdateAsync(activeIngredientDto, id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(
                await _activeIngredientService.DeleteAsync(id));
        }
    }
}
