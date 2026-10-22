using Pharmacy.System.Core.Dtos.ProductPackagingDtos;
using Pharmacy.System.Services.Responses;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface IProductPackagingService
    {
        public Task<Response> CreateAsync(int productId, ProductPackagingDto dto);
        public Task<Response> GetProductPackagingLevelsAsync(int productId);
        public Task<Response> UpdateAsync(int productId, int ProductPackagingId, UpdateProductPackagingDto dto);
        public Task<Response> DeleteAsync(int productId, int ProductPackagingId);
        public Task<Response> DeleteProductPackagingLevelsAsync(int productId);
        public Task<Response> GetProductBaseUnit(int productId);

        /// <summary>
        /// Used to configure a default packaging for a product.
        /// This default packaging will be used when returning product quantities in stock.
        /// </summary>
        /// <param name="productId">The targeted product that you want to configure default packaging for.</param>
        /// <param name="packagingLevelId">The packaging Level to be defaulte for the targeted product.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a failure response.</returns>
        public Task<Response> AddProductDefaultPackagingAsync(int productId, int packagingLevelId);

        /// <summary>
        /// Used to get the default packaging for a product.
        /// </summary>
        /// <param name="productId">The product that you want to get its default packaging.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a failure response.</returns>
        public Task<Response> GetProductDefaultPackagingAsync(int productId);

        /// <summary>
        /// Used to delete the default packaging for a product.
        /// </summary>
        /// <param name="productId">The product that you want to delete its default packaging.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a failure response.</returns>
        public Task<Response> DeleteProductDefaultPackagingAsync(int productId);


    }
}
