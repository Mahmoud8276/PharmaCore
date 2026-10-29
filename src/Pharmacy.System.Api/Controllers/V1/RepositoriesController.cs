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
            var response = await _repositoryService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] RepositorySpecParams specParams)
        {
            var response = await _repositoryService.GetAllAsync(specParams);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var response = await _repositoryService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] RepositoryDto dto)
        {
            var response = await _repositoryService.UpdateAsync(dto, id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var response = await _repositoryService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
