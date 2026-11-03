using Pharmacy.System.Core.Dtos.ProductBatchDtos;
using Pharmacy.System.Core.Models;
using Pharmacy.System.Services.SpecificationParams;

namespace Pharmacy.System.Services.IServices
{
    public interface IProductBatchService 
        : IWriteService<ProductBatch, int, ProductBatchDto, ProductBatchDto, ProductBatchSpecParams>
    {
    }
}
