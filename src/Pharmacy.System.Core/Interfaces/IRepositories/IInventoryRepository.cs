using Pharmacy.System.Core.Dtos;
using Pharmacy.System.Core.Interfaces.ISpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Core.Interfaces.IRepositories
{
    public interface IInventoryRepository
    {
        /// <summary>
        /// Retrieves a paginated, product-level view of the inventory, filtered by the
        /// supplied specification parameters.
        /// </summary>
        /// <param name="specParams">
        /// The specification parameters that control filtering and paging:
        /// <list type="bullet">
        ///   <item><description><c>RepositoryId</c>: only include stock held in this repository.</description></item>
        ///   <item><description><c>CategoryId</c>: only include products in this category.</description></item>
        ///   <item><description><c>Name</c>: only include products whose name contains this text.</description></item>
        ///   <item><description><c>Barcode</c>: only include products having this exact barcode.</description></item>
        ///   <item><description><c>PageIndex</c> / <c>PageSize</c>: 1-based page number and page size.</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// A <see cref="Pagination"/> object whose <c>Data</c> is a list of
        /// <see cref="InventoryRowDetailsDto"/> (one row per product, ordered by product name),
        /// and whose <c>Count</c> is the total number of distinct products that match the
        /// filters (not the number of rows on the current page). If the requested page contains
        /// no products, <c>Data</c> is an empty list.
        /// </returns>
        /// <remarks>
        /// <para>
        /// Only inventory batches with <c>OnHandBaseUnits &gt; 0</c> are considered.
        /// </para>
        /// <para>
        /// The method works in two steps: it first pages over the distinct product IDs
        /// so that paging is applied per product rather than per batch, then loads all
        /// batches of those products (with their related data) and groups them by product.
        /// </para>
        /// <para>
        /// For each product, the total on-hand base units across all batches is summed and
        /// compared with <c>Product.MinimumStockLevel</c>. The row's status is
        /// <see cref="InventoryStatus.Low"/> if the total is less than or equal to the minimum
        /// stock level, and <see cref="InventoryStatus.Normal"/> otherwise (including when no
        /// minimum is configured). The displayed quantity is produced by
        /// <see cref="CalculateQuantity(IEnumerable{InventoryItem})"/>.
        /// </para>
        /// </remarks>
        public Task<Pagination> GetAllInventoryWithSpecAsync(IInventorySpecParams specParams);
    }
}
