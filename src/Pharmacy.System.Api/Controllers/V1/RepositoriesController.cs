using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.System.Core.Dtos.RepositoryDtos;
using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class RepositoriesController : BaseController
    {
        private readonly IRepositoryService _repositoryService;
        public RepositoriesController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RepositoryDto dto)
        {
            return FromResponse(
                await _repositoryService.CreateAsync(dto));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] RepositorySpecParams specParams)
        {
            return FromResponse(
                await _repositoryService.GetAllAsync(specParams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            return FromResponse(
                await _repositoryService.GetByIdAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] RepositoryDto dto)
        {
            return FromResponse(
                await _repositoryService.UpdateAsync(dto, id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            return FromResponse(
                await _repositoryService.DeleteAsync(id));
        }
    }
}
