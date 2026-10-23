using Pharmacy.System.Core.Dtos.ProductActiveIngredientDtos;
using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Services.Responses;
using Pharmacy.System.Services.SpecificationParams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.System.Services.IServices
{
    public interface IProductService
    {
        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="dto">The product's core details (name, barcode, category, form, manufacturer, etc.).</param>
        /// <returns>A <see cref="Response"/> containing the created product on success, or a failure
        /// response if the barcode is already in use or a referenced manufacturer/category/form does not exist.</returns>
        public Task<Response> CreateAsync(ProductDto dto);

        /// <summary>
        /// Retrieves a filtered, sorted, and paginated list of products.
        /// </summary>
        /// <param name="specParams">Filtering, sorting, and pagination parameters.</param>
        /// <returns>A <see cref="Response"/> containing the matching products.</returns>
        public Task<Response> GetAllAsync(ProductSpecParams specParams);

        /// <summary>
        /// Retrieves a single product by its id.
        /// </summary>
        /// <param name="id">The product's id.</param>
        /// <returns>A <see cref="Response"/> containing the product, or a not-found response if no product
        /// with the given id exists.</returns>
        public Task<Response> GetByIdAsync(int id);

        /// <summary>
        /// Updates an existing product's core details.
        /// </summary>
        /// <param name="id">The id of the product to update.</param>
        /// <param name="dto">The updated product details.</param>
        /// <returns>A <see cref="Response"/> containing the updated product on success, or a failure
        /// response if the product does not exist or the barcode collides with a different product.</returns>
        public Task<Response> UpdateAsync(int id, UpdateProductDto dto);

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <param name="id">The id of the product to delete.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a conflict response if the product
        /// still has dependent data (packaging levels, stock, etc.) that prevents deletion.</returns>
        public Task<Response> DeleteAsync(int id);

        /// <summary>
        /// Used to update product image.
        /// </summary>
        /// <param name="productId">The target product for its image to be updated</param>
        /// <param name="dto">The DTO containing the new Image</param>
        /// <returns>A <see cref="Response"/> containing a success, or a failure
        /// response</returns>
        public Task<Response> UpdateProductImageAsync(int productId, UpdateProductImageDto dto);

        /// <summary>
        /// Used to delete product image.
        /// </summary>
        /// <param name="productId">The target product for its image to be deleted.</param>
        /// <returns>A <see cref="Response"/> containing a success, or a failure
        /// response</returns>
        public Task<Response> DeleteProductImageAsync(int productId);

        /// <summary>
        /// Generates a new unique barcode for a product.
        /// </summary>
        /// <returns>A <see cref="Response"/> containing a failure, or a success
        /// response containing the unique generated barcode.</returns>
        public Task<Response> GenerateBarcodeAsync();

        /// <summary>
        /// Associates a single active ingredient with a product.
        /// </summary>
        /// <param name="productId">The id of the product to associate the ingredient with.</param>
        /// <param name="activeIngredientId">The id of the active ingredient to associate.</param>
        /// <param name="dto">The strength/dosage details for this association.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a failure response if the product or
        /// active ingredient does not exist, or the association already exists.</returns>
        public Task<Response> AddProductActiveIngredientAsync(int productId, int activeIngredientId, ProductActiveIngredientDto dto);

        /// <summary>
        /// Associates multiple active ingredients with a product in a single call.
        /// </summary>
        /// <param name="productId">The id of the product to associate the ingredients with.</param>
        /// <param name="dtos">The active ingredients to associate, each with its own strength/dosage details.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a failure response if the product does
        /// not exist, an ingredient id is invalid, or an ingredient is already associated with the product.</returns>
        public Task<Response> AddProductActiveIngredientsAsync(int productId, List<ProductActiveIngredientAssociationDto> dtos);

        /// <summary>
        /// Retrieves all active ingredients currently associated with a product.
        /// </summary>
        /// <param name="productId">The id of the product.</param>
        /// <returns>A <see cref="Response"/> containing the product's associated active ingredients.</returns>
        public Task<Response> GetProductActiveIngredientsAsync(int productId);

        /// <summary>
        /// Updates the strength/dosage details of a single active ingredient already associated with a product.
        /// </summary>
        /// <param name="productId">The id of the product.</param>
        /// <param name="activeIngredientId">The id of the active ingredient whose association is being updated.</param>
        /// <param name="dto">The updated strength/dosage details.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a not-found response if the product or
        /// the association does not exist.</returns>
        public Task<Response> UpdateProductActiveIngredientAsync(int productId, int activeIngredientId, ProductActiveIngredientDto dto);

        /// <summary>
        /// Replaces the full set of active ingredients associated with a product.
        /// </summary>
        /// <remarks>
        /// This is a full replace (PUT semantics), not a merge: any ingredient currently associated with
        /// the product but absent from <paramref name="dtos"/> is removed. Ingredients present in both are
        /// updated; ingredients present only in <paramref name="dtos"/> are added.
        /// </remarks>
        /// <param name="productId">The id of the product.</param>
        /// <param name="dtos">The complete list of active ingredients the product should have after this call.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a failure response if the product does
        /// not exist, an ingredient id is invalid, or the list contains a duplicate ingredient id.</returns>
        public Task<Response> UpdateProductActiveIngredientsAsync(int productId, List<ProductActiveIngredientAssociationDto> dtos);

        /// <summary>
        /// Removes a single active ingredient association from a product.
        /// </summary>
        /// <param name="productId">The id of the product.</param>
        /// <param name="activeIngredientId">The id of the active ingredient to remove.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a not-found response if the product or
        /// the association does not exist.</returns>
        public Task<Response> DeleteProductActiveIngredientAsync(int productId, int activeIngredientId);

        /// <summary>
        /// Removes every active ingredient association from a product.
        /// </summary>
        /// <remarks>
        /// Destructive and irreversible for the product's recorded composition - clears every association
        /// in one call. Consider requiring explicit confirmation upstream (controller/UI) rather than
        /// treating this the same as removing a single ingredient.
        /// </remarks>
        /// <param name="productId">The id of the product.</param>
        /// <returns>A <see cref="Response"/> indicating success, or a not-found response if the product does not exist.</returns>
        public Task<Response> DeleteProductActiveIngredientsAsync(int productId);

    }
}