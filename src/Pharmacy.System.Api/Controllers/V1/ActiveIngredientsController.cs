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
            var result = await _activeIngredientService.CreateAsync(activeIngredientDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] ActiveIngredientSpecParams specParams)
        {
            var result = await _activeIngredientService.GetAllAsync(specParams);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _activeIngredientService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] ActiveIngredientDto activeIngredientDto)
        {
            var result = await _activeIngredientService.UpdateAsync(activeIngredientDto, id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _activeIngredientService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
