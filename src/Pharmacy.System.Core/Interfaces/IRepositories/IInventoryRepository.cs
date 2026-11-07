using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Dtos.InventoryDtos;
using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using Pharmacy.System.Core.Models;
using System.Threading.Tasks;

namespace Pharmacy.System.Core.Interfaces.IRepositories
{
    public interface IInventoryRepository
    {
        /// <summary>Gets a paginated list of distinct products that currently have stock.</summary>
        /// <remarks>
        /// Excludes inventory items with zero on-hand quantity. Supports filtering by
        /// repository, category, product name, and barcode. Products are grouped across
        /// their batches and repositories, and each result contains product details,
        /// aggregated on-hand quantity, and stock status. Status is undefined when the
        /// product has no configured minimum stock level.
        /// </remarks>
        /// <param name="specParams">Filter and paging criteria. <c>PageIndex</c> is one-based.</param>
        /// <returns>A <see cref="Pagination"/> containing inventory rows and the total number of distinct matching products before paging.</returns>
        public Task<Pagination> GetInventoryProductsAsync(IInventorySpecParams specParams);


        /// <summary>Gets the overall stock summary for a product across all repositories.</summary>
        /// <remarks>
        /// Includes total on-hand quantity, stock status, quantity per repository, and
        /// total selling value. Quantity formatting uses each supply item's packaging
        /// snapshot. Items without on-hand stock are excluded. If the product is null
        /// or has no matching stock, the returned DTO uses <c>--</c> for total quantity.
        /// Total purchase cost is not implemented yet and is currently returned as zero.
        /// </remarks>
        /// <param name="product">The product to summarize.</param>
        /// <returns>A <see cref="ProductStockDetailsDto"/> containing the product's stock overview.</returns>
        public Task<ProductStockDetailsDto> GetProductInventoryOverviewAsync(Product product);


        /// <summary>Gets a paginated list of the stock groups belonging to a product.</summary>
        /// <remarks>
        /// Includes only stock groups with positive on-hand quantity. Supports filtering
        /// by repository, supplier, batch number, and expiry-date range. Results are
        /// ordered by expiry date and then batch number, with earliest expiry first.
        /// Each DTO contains remaining and originally received quantities formatted using
        /// the packaging snapshot for that inventory item's supply record.
        /// </remarks>
        /// <param name="product">The product whose stock groups should be retrieved.</param>
        /// <param name="specParams">Filter and paging criteria. Expiry-date bounds are inclusive; <c>PageIndex</c> is one-based.</param>
        /// <returns>A <see cref="Pagination"/> containing the requested stock-group page and the total number of filtered groups.</returns>
        public Task<Pagination> GetProductStockGroupsAsync(Product product, IProductStockGroupSpecParams specParams);
    }
}
