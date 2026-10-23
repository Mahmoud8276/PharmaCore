using Microsoft.Extensions.Logging;
using Moq;
using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Services.Services;
using AutoFixture;
using Pharmacy.System.Core.Dtos.ProductDtos;
using System.Linq.Expressions;
using Pharmacy.System.Core.Models;
using Microsoft.AspNetCore.Http;
using Pharmacy.System.Services.Specifications;
using FluentAssertions;
using System.Net;
using Pharmacy.System.Services.IServices;

namespace Pharmacy.System.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly Fixture _fixture;
        

        private readonly ProductService _sut;

        public ProductServiceTests()
        {
            _fixture = new Fixture();

            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            _sut = new ProductService(
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }


        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_ValidProduct_ReturnsSucessCreationResponse()
        {
            // Arrange
            var productDto = _fixture.Build<ProductDto>()
                .With(x=>x.ManufacturerId, (int?)null)
                .With(x=>x.Image, (IFormFile?)null)
                .Create();

            Product? addedProduct = null; 

            _unitOfWorkMock.Setup(uow => uow.ProductRepository
            .AnyAsync(It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository
            .AnyAsync(It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                .ReturnsAsync(false);
            
            _unitOfWorkMock.Setup(uow => uow.ProductCategoryRepository
            .AnyAsync(It.IsAny<Expression<Func<ProductCategory, bool>>>()))
                .ReturnsAsync(true);
            
            _unitOfWorkMock.Setup(uow => uow.ProductFormRepository
            .AnyAsync(It.IsAny<Expression<Func<ProductForm, bool>>>()))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.ProductRepository
            .AddAsync(It.IsAny<Product>()))
                .Callback<Product>(
                p =>{
                    p.Id = 1;
                    addedProduct = p;
                })
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetWithSpecAsync(
                It.IsAny<ProductSpecification>())).ReturnsAsync(() => addedProduct);

            // Act
            var result = await _sut.CreateAsync(productDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
            result.Message.Should().Be("Product created successfully");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<ProductDetailsDto>()
                .Which.Id.Should().Be(1);

            addedProduct.Should().NotBeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.AddAsync(It.IsAny<Product>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateProductBarcodes_ReturnsBadRequestResponse()
        {
            //Arrange
            var productDto = _fixture.Build<ProductDto>()
                .With(x=>x.Barcodes, new List<string>() { "111111", "111111" })
                .With(x => x.Image, (IFormFile?)null)
                .Create();

            //Act
            var result = await _sut.CreateAsync(productDto);

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            result.Message.Should().Be("Duplicate barcodes in the request!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.AddAsync(It.IsAny<Product>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ProductNameAlreadyExists_ReturnsConflictResponse()
        {
            //Arrange
            var productDto = _fixture.Build<ProductDto>()
                .With(x => x.Image, (IFormFile?)null)
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(true);
            
            //Act
            var result = await _sut.CreateAsync(productDto);
            
            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("Product name already exists!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.AddAsync(It.IsAny<Product>()),
                Times.Never);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductCategoryRepository.AnyAsync(
                    It.IsAny<Expression<Func<ProductCategory, bool>>>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_BarcodeAlreadyExists_ReturnsConflictResponse()
        {
            //Arrange
            var productDto = _fixture.Build<ProductDto>()
                .With(x=>x.Image, (IFormFile?)null)
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository
                .AnyAsync(It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                    .ReturnsAsync(true);

            //Act
            var result = await _sut.CreateAsync(productDto);

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("One or more product barcode already exists!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.AddAsync(It.IsAny<Product>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.ProductCategoryRepository.AnyAsync(
                    It.IsAny<Expression<Func<ProductCategory, bool>>>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ManufacturerDoesNotExist_ReturnsNotFoundResponse()
        {
            //Arrange
            var productDto = _fixture.Build<ProductDto>()
                .With(x => x.Image, (IFormFile?)null)
                .With(x => x.ManufacturerId, 12)
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);
            
            _unitOfWorkMock.Setup(uow => uow.ManufacturerRepository.AnyAsync(
                It.IsAny<Expression<Func<Manufacturer, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository
                .AnyAsync(It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                    .ReturnsAsync(false);

            //Act
            var result = await _sut.CreateAsync(productDto);

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Manufacturer not found!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.AddAsync(It.IsAny<Product>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.ProductCategoryRepository.AnyAsync(
                    It.IsAny<Expression<Func<ProductCategory, bool>>>()),
                Times.Never);

        }

        [Fact]
        public async Task CreateAsync_ProductCategoryDoesNotExist_ReturnsNotFoundResponse()
        {
            //Arrange
            var productDto = _fixture.Build<ProductDto>()
                .With(x => x.ManufacturerId, (int?)null)
                .With(x => x.Image, (IFormFile?)null)
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductCategoryRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductCategory, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository
                .AnyAsync(It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                    .ReturnsAsync(false);

            //Act
            var result = await _sut.CreateAsync(productDto);

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Product category not found!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.AddAsync(It.IsAny<Product>()),
                Times.Never);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductFormRepository.AnyAsync(
                    It.IsAny<Expression<Func<ProductForm, bool>>>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ProductFormDoesNotExist_ReturnsNotFoundResponse()
        {
            //Arrange
            var productDto = _fixture.Build<ProductDto>()
                .With(x => x.ManufacturerId, (int?)null)
                .With(x => x.Image, (IFormFile?)null)
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);
            
            _unitOfWorkMock.Setup(uow => uow.ProductCategoryRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductCategory, bool>>>()))
                .ReturnsAsync(true);
            
            _unitOfWorkMock.Setup(uow => uow.ProductFormRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductForm, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository
                .AnyAsync(It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                    .ReturnsAsync(false);

            //Act
            var result = await _sut.CreateAsync(productDto);
            
            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Product form not found!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.AddAsync(It.IsAny<Product>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        #endregion


        #region DeleteAsyncTests

        [Fact]
        public async Task DeleteAsync_ValidDeletion_ReturnsSucessResponse()
        {
            // Arrange
            int productId = 1;
            Product productToDelete = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Image, (string?)null)
                .With(p =>p.Barcodes, new List<ProductBarcode>())
                .With(p =>p.DefaultPackaging, new ProductPackagingLevel())
                .With(p =>p.Manufacturer, (Manufacturer?)null)
                .With(p =>p.ProductForm, (ProductForm?)null)
                .With(p =>p.Category, (ProductCategory?)null)
                .With(p => p.ProductActiveIngredients, new HashSet<ProductActiveIngredient>())
                .With(p => p.ProductPackagingLevels, new HashSet<ProductPackagingLevel>())
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync(productToDelete);
            
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.Delete(productToDelete));
            
            _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteAsync(productId);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Message.Should().Be("Product deleted successfully");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetByIdAsync(productId),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.Delete(productToDelete),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Once);
        }


        [Fact]
        public async Task DeleteAsync_ProductNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            int productId = 1;
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _sut.DeleteAsync(productId);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Product not found!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetByIdAsync(productId),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.Delete(It.IsAny<Product>()),
                Times.Never);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        #endregion


        #region GetByIdAsyncTests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsProductDetails()
        {
            // Arrange
            int productId = 1;
            Product product = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Image, (string?)null)
                .With(p => p.Barcodes, new List<ProductBarcode>())
                .With(p => p.DefaultPackaging, new ProductPackagingLevel())
                .With(p => p.Manufacturer, (Manufacturer?)null)
                .With(p => p.ProductForm, (ProductForm?)null)
                .With(p => p.Category, (ProductCategory?)null)
                .With(p => p.ProductActiveIngredients, new HashSet<ProductActiveIngredient>())
                .With(p => p.ProductPackagingLevels, new HashSet<ProductPackagingLevel>())
                .Create();


            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetWithSpecAsync(
                It.IsAny<ProductSpecification>()))
                .ReturnsAsync(product);

            // Act
            var result = await _sut.GetByIdAsync(productId);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Message.Should().Be("Product retrieved successfully");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<ProductDetailsDto>()
                .Which.Id.Should().Be(productId);
           
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetWithSpecAsync(
                    It.IsAny<ProductSpecification>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ProductNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            int productId = 1;
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetWithSpecAsync(
                It.IsAny<ProductSpecification>()))
                .ReturnsAsync((Product?)null);
            
            // Act
            var result = await _sut.GetByIdAsync(productId);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Product not found!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetWithSpecAsync(
                    It.IsAny<ProductSpecification>()),
                Times.Once);
        }

        #endregion


        #region UpdateAsyncTests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResponse()
        {
            //Arrange
            int productId = 1;
            var productDto = _fixture.Build<UpdateProductDto>()
                .With(x=>x.ManufacturerId, (int?)null)
                .Create();

            Product existingProduct = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Image, (string?)null)
                .With(p => p.Barcodes, new List<ProductBarcode>())
                .With(p => p.DefaultPackaging, new ProductPackagingLevel())
                .With(p => p.Manufacturer, (Manufacturer?)null)
                .With(p => p.ProductForm, (ProductForm?)null)
                .With(p => p.Category, (ProductCategory?)null)
                .With(p => p.ProductActiveIngredients, new HashSet<ProductActiveIngredient>())
                .With(p => p.ProductPackagingLevels, new HashSet<ProductPackagingLevel>())
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetByIdAsync(
                It.IsAny<int>()))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow=>uow.ProductRepository.GetWithSpecAsync(
                It.IsAny<ProductSpecification>()))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductCategoryRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductCategory, bool>>>()))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.ProductFormRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductForm, bool>>>()))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.CompleteAsync())
                .Returns(Task.CompletedTask);

            //Act
            var result = await _sut.UpdateAsync(productId, productDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Message.Should().Be("Product updated successfully");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<ProductDetailsDto>()
                .Which.Id.Should().Be(productId);
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetWithSpecAsync(
                    It.IsAny<ProductSpecification>()),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ProductNotFound_ReturnsNotFoundResponse()
        {
            //Arrange
            int productId = 1;
            var productDto = _fixture.Build<UpdateProductDto>()
                .With(x => x.ManufacturerId, (int?)null)
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetByIdAsync(
                It.IsAny<int>()))
                .ReturnsAsync((Product)null);

            //Act
            var result = await _sut.UpdateAsync(productId, productDto);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Product not found!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetByIdAsync(
                    It.IsAny<int>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_BarcodeAlreadyExists_ReturnsConflictResponse()
        {
            //Arrange
            int productId = 1;
            var productDto = _fixture.Build<UpdateProductDto>()
                .With(x => x.ManufacturerId, (int?)null)
                .Create();

            Product existingProduct = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Image, (string?)null)
                .With(p => p.Barcodes, new List<ProductBarcode>())
                .With(p => p.DefaultPackaging, new ProductPackagingLevel())
                .With(p => p.Manufacturer, (Manufacturer?)null)
                .With(p => p.ProductForm, (ProductForm?)null)
                .With(p => p.Category, (ProductCategory?)null)
                .With(p => p.ProductActiveIngredients, new HashSet<ProductActiveIngredient>())
                .With(p => p.ProductPackagingLevels, new HashSet<ProductPackagingLevel>())
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetByIdAsync(
                It.IsAny<int>()))
                .ReturnsAsync(existingProduct);
            
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                .ReturnsAsync(true);

            //Act
            var result = await _sut.UpdateAsync(productId, productDto);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("One or more product barcode already exists!");
            result.Data.Should().BeNull();
           
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetByIdAsync(
                    It.IsAny<int>()),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ManufacturerDoesNotExist_ReturnsNotFoundResponse()
        {
            //Arrange
            int productId = 1;
            var productDto = _fixture.Build<UpdateProductDto>()
                .With(x => x.ManufacturerId, 12)
                .Create();

            Product existingProduct = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Image, (string?)null)
                .With(p => p.Barcodes, new List<ProductBarcode>())
                .With(p => p.DefaultPackaging, new ProductPackagingLevel())
                .With(p => p.Manufacturer, (Manufacturer?)null)
                .With(p => p.ProductForm, (ProductForm?)null)
                .With(p => p.Category, (ProductCategory?)null)
                .With(p => p.ProductActiveIngredients, new HashSet<ProductActiveIngredient>())
                .With(p => p.ProductPackagingLevels, new HashSet<ProductPackagingLevel>())
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetByIdAsync(
                It.IsAny<int>()))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ManufacturerRepository.AnyAsync(
                It.IsAny<Expression<Func<Manufacturer, bool>>>()))
                .ReturnsAsync(false);
            
            //Act
            var result = await _sut.UpdateAsync(productId, productDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Manufacturer not found!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetByIdAsync(
                    It.IsAny<int>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ProductCategoryDoesNotExist_ReturnsNotFoundResponse()
        {
            //Arrange
            int productId = 1;
            var productDto = _fixture.Build<UpdateProductDto>()
                .With(x => x.ManufacturerId, (int?)null)
                .Create();

            Product existingProduct = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Image, (string?)null)
                .With(p => p.Barcodes, new List<ProductBarcode>())
                .With(p => p.DefaultPackaging, new ProductPackagingLevel())
                .With(p => p.Manufacturer, (Manufacturer?)null)
                .With(p => p.ProductForm, (ProductForm?)null)
                .With(p => p.Category, (ProductCategory?)null)
                .With(p => p.ProductActiveIngredients, new HashSet<ProductActiveIngredient>())
                .With(p => p.ProductPackagingLevels, new HashSet<ProductPackagingLevel>())
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetByIdAsync(
                It.IsAny<int>()))
                .ReturnsAsync(existingProduct);
            
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);
            
            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                .ReturnsAsync(false);
            
            _unitOfWorkMock.Setup(uow => uow.ProductCategoryRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductCategory, bool>>>()))
                .ReturnsAsync(false);

            //Act
            var result = await _sut.UpdateAsync(productId, productDto);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Product category not found!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetByIdAsync(
                    It.IsAny<int>()),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ProductFormDoesNotExist_ReturnsNotFoundResponse()
        {
            //Arrange
            int productId = 1;
            var productDto = _fixture.Build<UpdateProductDto>()
                .With(x => x.ManufacturerId, (int?)null)
                .Create();

            Product existingProduct = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.Image, (string?)null)
                .With(p => p.Barcodes, new List<ProductBarcode>())
                .With(p => p.DefaultPackaging, new ProductPackagingLevel())
                .With(p => p.Manufacturer, (Manufacturer?)null)
                .With(p => p.ProductForm, (ProductForm?)null)
                .With(p => p.Category, (ProductCategory?)null)
                .With(p => p.ProductActiveIngredients, new HashSet<ProductActiveIngredient>())
                .With(p => p.ProductPackagingLevels, new HashSet<ProductPackagingLevel>())
                .Create();

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetByIdAsync(
                It.IsAny<int>()))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.ProductRepository.AnyAsync(
                It.IsAny<Expression<Func<Product, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductBarcodeRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductBarcode, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.ProductCategoryRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductCategory, bool>>>()))
                .ReturnsAsync(true);

            _unitOfWorkMock.Setup(uow => uow.ProductFormRepository.AnyAsync(
                It.IsAny<Expression<Func<ProductForm, bool>>>()))
                .ReturnsAsync(false);
            
            //Act
            var result = await _sut.UpdateAsync(productId, productDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Product form not found!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.ProductRepository.GetByIdAsync(
                    It.IsAny<int>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        #endregion
    }
}
