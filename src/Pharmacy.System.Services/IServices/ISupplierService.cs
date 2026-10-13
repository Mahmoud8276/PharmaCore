using Pharmacy.System.Core.Dtos.SupplierDtos;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    /// <summary>
    /// Defines the operations for managing suppliers, the parties that
    /// deliver supplies (stock) to the pharmacy.
    /// </summary>
    /// <remarks>
    /// All methods return a <see cref="Response"/> that wraps the outcome of the
    /// operation (success or failure, a message, and the data when applicable),
    /// so callers do not need to handle exceptions for expected business failures
    /// such as "supplier not found".
    /// </remarks>
    public interface ISupplierService
    {
        /// <summary>
        /// Creates a new supplier.
        /// </summary>
        /// <param name="dto">The supplier data to create, including name and contact details.</param>
        /// <returns>
        /// A <see cref="Response"/> containing the created supplier on success,
        /// or the validation or business error on failure.
        /// </returns>
        public Task<Response> CreateAsync(SupplierDto dto);

        /// <summary>
        /// Retrieves a single supplier by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the supplier.</param>
        /// <returns>
        /// A <see cref="Response"/> containing the supplier when found,
        /// or a not-found result when no supplier has the given <paramref name="id"/>.
        /// </returns>
        public Task<Response> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a list of suppliers, optionally filtered, sorted and paged.
        /// </summary>
        /// <param name="specParams">
        /// The query options (search text, sorting, page index and page size)
        /// used to build the specification for the query.
        /// </param>
        /// <returns>
        /// A <see cref="Response"/> containing the matching suppliers
        /// (an empty collection when nothing matches).
        /// </returns>
        public Task<Response> GetAllAsync(SupplierSpecParams specParams);

        /// <summary>
        /// Updates the details of an existing supplier.
        /// </summary>
        /// <param name="id">The unique identifier of the supplier to update.</param>
        /// <param name="dto">The new supplier data that replaces the current values.</param>
        /// <returns>
        /// A <see cref="Response"/> containing the updated supplier on success,
        /// or a not-found or validation error on failure.
        /// </returns>
        public Task<Response> UpdateAsync(int id, SupplierDto dto);

        /// <summary>
        /// Deletes a supplier.
        /// </summary>
        /// <param name="id">The unique identifier of the supplier to delete.</param>
        /// <returns>
        /// A <see cref="Response"/> indicating whether the deletion succeeded,
        /// or a not-found or business error on failure.
        /// </returns>
        /// <remarks>
        /// A supplier that already has supplies, payments or returns should not be
        /// permanently removed, because that history must stay readable. Implementations
        /// should prefer a soft delete or reject the request in that case.
        /// </remarks>
        public Task<Response> DeleteAsync(int id);
    }
}