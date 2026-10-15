using Pharmacy.System.Core.Interfaces;
using Moq;
using Pharmacy.System.Services.Services;
using Microsoft.Extensions.Logging;
using AutoFixture;
using Pharmacy.System.Core.Dtos.SupplierDtos;
using System.Linq.Expressions;
using Pharmacy.System.Core.Models;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Pharmacy.System.Services.Specifications;

namespace Pharmacy.System.UnitTests.Services
{
    public class SupplierServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<SupplierService>> _loggerMock;
        private readonly Fixture _fixture;

        private readonly SupplierService _sut;
        public SupplierServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<SupplierService>>();
            _fixture = new Fixture();

            _sut = new SupplierService(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        private void SetupExistingSuppliers(params Supplier[] existing)
        {
            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.AnyAsync(
                    It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync((Expression<Func<Supplier, bool>> predicate) =>
                    existing.Any(predicate.Compile()));
        }


        #region CreateAsyncTests

        [Fact]
        public async Task CreateAsync_ValidSupplierDto_ReturnsCreatedResponse()
        {
            // Arrange
            var supplierDto = _fixture.Build<SupplierDto>()
                .With(x=>x.Logo, (IFormFile?)null)
                .With(x=>x.PhoneNumbers, new List<string>() { "12345678912" })
                .Create();

            Supplier? addedSupplier = null;


            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.AnyAsync(
                It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository
                .AddAsync(It.IsAny<Supplier>()))
                    .Callback<Supplier>(
                    s => {
                        s.Id = 1;
                        addedSupplier = s;
                    })
                    .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.CompleteAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CreateAsync(supplierDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
            result.Message.Should().Be("Supplier created successfully");
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<SupplierDetailsDto>()
                .Which.Id.Should().Be(1);

            addedSupplier.Should().NotBeNull();

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AddAsync(It.IsAny<Supplier>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateSupplierName_ReturnsConflictResponse()
        {
            // Arrange
            var supplierDto = _fixture.Build<SupplierDto>()
                .With(x => x.Logo, (IFormFile?)null)
                .With(x => x.PhoneNumbers, new List<string>() { "12345678912" })
                .Create();


            SetupExistingSuppliers(new Supplier { Name = supplierDto.Name });

            // Act
            var result = await _sut.CreateAsync(supplierDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("Supplier with the same name already exists!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AddAsync(It.IsAny<Supplier>()),
                Times.Never);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DuplicateEmail_ReturnsConflictResponse()
        {
            // Arrange
            var supplierDto = _fixture.Build<SupplierDto>()
                .With(x => x.Logo, (IFormFile?)null)
                .With(x => x.PhoneNumbers, new List<string>() { "12345678912" })
                .Create();
            
            SetupExistingSuppliers(new Supplier { 
                Name = "Any Name",
                Email = supplierDto.Email });
            
            // Act
            var result = await _sut.CreateAsync(supplierDto);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("Supplier with the same email already exists!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AddAsync(It.IsAny<Supplier>()),
                Times.Never);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DuplicatePhoneNumber_ReturnsConflictResponse()
        {
            // Arrange
            var supplierDto = _fixture.Build<SupplierDto>()
                .With(x => x.Logo, (IFormFile?)null)
                .With(x => x.PhoneNumbers, new List<string>() { "12345678912" })
                .Create();

            SetupExistingSuppliers(new Supplier
            {
                Name = "Any Name",
                PhoneNumbers = new List<SupplierPhoneNumber> { 
                    new SupplierPhoneNumber { 
                        Number = "12345678912"
                    }
                }
            });

            // Act
            var result = await _sut.CreateAsync(supplierDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("One or more phone numbers already belongs to a supplier!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AddAsync(It.IsAny<Supplier>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        #endregion

        #region DeleteAsyncTests

        [Fact]
        public async Task DeleteAsync_ValidSupplierToDelete_ReturnsDeletedResponse()
        {
            // Arrange
            var supplierId = 1;
            
            var existingSupplier = _fixture.Build<Supplier>()
                .With(s => s.Id, supplierId)
                .With(s => s.Logo, (string?)null)
                .Create();
            
            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetByIdAsync(supplierId))
                .ReturnsAsync(existingSupplier);

            _unitOfWorkMock.Setup(uow => uow.SupplyRepository.AnyAsync(
                It.IsAny<Expression<Func<Supply, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.Delete(existingSupplier));
            
            _unitOfWorkMock.Setup(uow => uow.CompleteAsync())
                .Returns(Task.CompletedTask);
            
            // Act
            var result = await _sut.DeleteAsync(supplierId);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Message.Should().Be("Supplier deleted successfully");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.Delete(existingSupplier),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SupplierDoesNotExist_ReturnsNotFoundResponse()
        {
            // Arrange
            var supplierId = 1;

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetByIdAsync(supplierId))
                .ReturnsAsync((Supplier?)null);

            // Act
            var result = await _sut.DeleteAsync(supplierId);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Supplier not found!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.Delete(It.IsAny<Supplier>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_SupplierWithExistingSupplies_ReturnsConflictResponse()
        {
            // Arrange
            var supplierId = 1;
            
            var existingSupplier = _fixture.Build<Supplier>()
                .With(s => s.Id, supplierId)
                .With(s => s.Logo, (string?)null)
                .Create();
            
            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetByIdAsync(supplierId))
                .ReturnsAsync(existingSupplier);
            
            _unitOfWorkMock.Setup(uow => uow.SupplyRepository.AnyAsync(
                It.IsAny<Expression<Func<Supply, bool>>>()))
                .ReturnsAsync(true);
            
            // Act
            var result = await _sut.DeleteAsync(supplierId);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("Cannot delete supplier with existing supplies!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.Delete(It.IsAny<Supplier>()),
                Times.Never);
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsDbUpdateExceptionOnComplete_ReturnsConflictResponse()
        {
            // Arrange
            var supplierId = 1;

            var existingSupplier = _fixture.Build<Supplier>()
                .With(s => s.Id, supplierId)
                .With(s => s.Logo, (string?)null)
                .Create();

            var ex = new DbUpdateException();

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetByIdAsync(supplierId))
                .ReturnsAsync(existingSupplier);

            _unitOfWorkMock.Setup(uow => uow.SupplyRepository.AnyAsync(
                It.IsAny<Expression<Func<Supply, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.Delete(existingSupplier));
            _unitOfWorkMock.Setup(uow => uow.CompleteAsync()).Throws(ex);

            //Act
            var result = await _sut.DeleteAsync(supplierId);

            //Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("Cannot delete this supplier, because it is referenced by other records!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.Delete(It.IsAny<Supplier>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) =>
                        state.ToString()!.Contains(
                            $"Delete blocked by FK for supplier {supplierId}")),
                    It.Is<Exception?>(e => ReferenceEquals(e, ex)),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ExistingSupplier_ReturnsSupplierDetails()
        {
            // Arrange
            var supplierId = 1;
            
            var existingSupplier = _fixture.Build<Supplier>()
                .With(s => s.Id, supplierId)
                .With(s => s.Logo, (string?)null)
                .Create();
            
            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()))
                .ReturnsAsync(existingSupplier);
            
            // Act
            var result = await _sut.GetByIdAsync(supplierId);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Message.Should().Be("Supplier data retrieved Successfully");
            result.Data.Should().NotBeNull();
            var supplierDetails = result.Data.Should().BeOfType<SupplierDetailsDto>()
                .Subject;

            supplierDetails.Id.Should().Be(supplierId);
            supplierDetails.Name.Should().Be(existingSupplier.Name);
            supplierDetails.Email.Should().Be(existingSupplier.Email);
            supplierDetails.Address.Should().Be(existingSupplier.Address);

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_SupplierDoesNotExist_ReturnsNotFoundResponse()
        {
            // Arrange
            var supplierId = 1;
           
            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()))
                .ReturnsAsync((Supplier?)null);
            
            // Act
            var result = await _sut.GetByIdAsync(supplierId);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Supplier not found!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()),
                Times.Once);
        }

        #endregion

        #region UpdateAsyncTests

        public static IEnumerable<object[]> PhoneNumberCases()
        {
            var fixture = new Fixture();

            yield return new object[]
            {
                1,
                fixture.Build<UpdateSupplierDto>()
                    .With(s => s.PhoneNumbers, new List<string>
                    {
                        "11111111111", "22222222222"
                    })
                    .Create(),
                fixture.Build<Supplier>()
                    .With(s => s.Id, 1)
                    .With(s => s.Logo, (string?)null)
                    .With(s => s.PhoneNumbers, new List<SupplierPhoneNumber>
                    {
                        new() { Number = "11111111111" },
                        new() { Number = "22222222222" }
                    })
                    .Create()
            };

            yield return new object[]
            {
                1,
                fixture.Build<UpdateSupplierDto>()
                    .With(s => s.PhoneNumbers, new List<string>
                    {
                        "33333333333", "44444444444"
                    })
                    .Create(),
                fixture.Build<Supplier>()
                    .With(s => s.Id, 1)
                    .With(s => s.Logo, (string?)null)
                    .With(s => s.PhoneNumbers, new List<SupplierPhoneNumber>
                    {
                        new() { Number = "11111111111" },
                        new() { Number = "22222222222" }
                    })
                    .Create()
            };

            yield return new object[]
            {
                1,
                fixture.Build<UpdateSupplierDto>()
                    .With(s => s.PhoneNumbers, new List<string>
                    {
                        "11111111111", "22222222222", "33333333333"
                    })
                    .Create(),
                fixture.Build<Supplier>()
                    .With(s => s.Id, 1)
                    .With(s => s.Logo, (string?)null)
                    .With(s => s.PhoneNumbers, new List<SupplierPhoneNumber>
                    {
                        new() { Number = "11111111111" },
                        new() { Number = "22222222222" }
                    })
                    .Create()
            };

            yield return new object[]
            {
                1,
                fixture.Build<UpdateSupplierDto>()
                    .With(s => s.PhoneNumbers, new List<string>
                    {
                        "11111111111"
                    })
                    .Create(),
                fixture.Build<Supplier>()
                    .With(s => s.Id, 1)
                    .With(s => s.Logo, (string?)null)
                    .With(s => s.PhoneNumbers, new List<SupplierPhoneNumber>
                    {
                        new() { Number = "11111111111" },
                        new() { Number = "22222222222" }
                    })
                    .Create()
            };

            yield return new object[]
            {
                1,
                fixture.Build<UpdateSupplierDto>()
                    .With(s => s.PhoneNumbers, new List<string>
                    {
                        "77777777777"
                    })
                    .Create(),
                fixture.Build<Supplier>()
                    .With(s => s.Id, 1)
                    .With(s => s.Logo, (string?)null)
                    .With(s => s.PhoneNumbers, new List<SupplierPhoneNumber>
                    {
                        new() { Number = "11111111111" },
                        new() { Number = "22222222222" }
                    })
                    .Create()
            };
        }


        [Theory]
        [MemberData(nameof(PhoneNumberCases))]
        public async Task UpdateAsync_ValidSupplierDto_ReturnsUpdatedResponse(
            int supplierId,
            UpdateSupplierDto updatedSupplierDto,
            Supplier existingSupplier)
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetWithSpecAsync(
                It.IsAny<SupplierSpecification>()))
                .ReturnsAsync(existingSupplier);
            
            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.AnyAsync(
                    It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync(false);
            
            _unitOfWorkMock.Setup(uow => uow.CompleteAsync())
                .Returns(Task.CompletedTask);
            
            // Act
            var result = await _sut.UpdateAsync(supplierId, updatedSupplierDto);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Message.Should().Be("Supplier updated successfully");
            result.Data.Should().NotBeNull();
            var supplierDetails = result.Data.Should().BeOfType<SupplierDetailsDto>()
                .Subject;

            supplierDetails.Id.Should().Be(supplierId);
            supplierDetails.Name.Should().Be(updatedSupplierDto.Name);
            supplierDetails.Description.Should().Be(updatedSupplierDto.Description);
            supplierDetails.Address.Should().Be(updatedSupplierDto.Address);
            supplierDetails.PhoneNumbers.Should().NotBeNull();
            supplierDetails.PhoneNumbers.Should().BeEquivalentTo(updatedSupplierDto.PhoneNumbers);

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AnyAsync(It.IsAny<Expression<Func<Supplier, bool>>>()),
                Times.Exactly(3)); // For name, email, and phone number checks
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SupplierNameAlreadyExists_ReturnsConflictResponse()
        {
            // Arrange
            var supplierId = 1;

            var updatedSupplierDto = _fixture.Build<UpdateSupplierDto>()
                .With(s => s.PhoneNumbers, new List<string> { "12345678912" })
                .Create();

            SetupExistingSuppliers(new Supplier()
            {
                Id = 2,
                Name = updatedSupplierDto.Name
            });

            // Act
            var result = await _sut.UpdateAsync(supplierId, updatedSupplierDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("Supplier with the same name already exists!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AnyAsync(It.IsAny<Expression<Func<Supplier, bool>>>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SupplierEmailAlreadyExists_ReturnsConflictResponse()
        {
            // Arrange
            var supplierId = 1;

            var updatedSupplierDto = _fixture.Build<UpdateSupplierDto>()
                .With(s => s.PhoneNumbers, new List<string> { "12345678912" })
                .Create();

            SetupExistingSuppliers(new Supplier()
            {
                Id = 2,
                Email = updatedSupplierDto.Email
            });

            // Act
            var result = await _sut.UpdateAsync(supplierId, updatedSupplierDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("Supplier with the same email already exists!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AnyAsync(It.IsAny<Expression<Func<Supplier, bool>>>()),
                Times.Exactly(2));

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SupplierPhoneNumberAlreadyExists_ReturnsConflictResponse()
        {
            // Arrange
            var supplierId = 1;

            var updatedSupplierDto = _fixture.Build<UpdateSupplierDto>()
                .With(s => s.PhoneNumbers, new List<string> { "11111111111" })
                .Create();

            SetupExistingSuppliers(new Supplier()
            {
                Id = 2,
                PhoneNumbers = new List<SupplierPhoneNumber>() { 
                    new SupplierPhoneNumber()
                    {
                        Number = "11111111111"
                    }
                }
            });

            // Act
            var result = await _sut.UpdateAsync(supplierId, updatedSupplierDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("One or more phone numbers already belongs to a supplier!");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AnyAsync(It.IsAny<Expression<Func<Supplier, bool>>>()),
                Times.Exactly(3));

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SupplierNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var supplierId = 1;
            
            var updatedSupplierDto = _fixture.Build<UpdateSupplierDto>()
                .With(s => s.PhoneNumbers, new List<string> { "12345678912" })
                .Create();

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.AnyAsync(
                It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetWithSpecAsync(
                It.IsAny<SupplierSpecification>()))
                .ReturnsAsync((Supplier?)null);
            
            // Act
            var result = await _sut.UpdateAsync(supplierId, updatedSupplierDto);
            
            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Message.Should().Be("Supplier not found!");
            result.Data.Should().BeNull();
            
            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()),
                Times.Once);
            
            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AnyAsync(It.IsAny<Expression<Func<Supplier, bool>>>()),
                Times.Exactly(3));
            
            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsDbUpdateExceptionOnComplete_ReturnsConflictResponse()
        {
            // Arrange
            var supplierId = 1;

            var updatedSupplierDto = _fixture.Build<UpdateSupplierDto>()
                .With(s => s.PhoneNumbers, new List<string> { "12345678912" })
                .Create();

            var existingSupplier = _fixture.Build<Supplier>()
                    .With(s => s.Id, supplierId)
                    .With(s => s.Logo, (string?)null)
                    .With(s => s.PhoneNumbers, new List<SupplierPhoneNumber> { new() { Number = "11111111111" } })
                    .Create();

            var ex = new DbUpdateException();

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.GetWithSpecAsync(
                It.IsAny<SupplierSpecification>()))
                .ReturnsAsync(existingSupplier);

            _unitOfWorkMock.Setup(uow => uow.SupplierRepository.AnyAsync(
                    It.IsAny<Expression<Func<Supplier, bool>>>()))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(uow => uow.CompleteAsync())
                .ThrowsAsync(ex);

            // Act
            var result = await _sut.UpdateAsync(supplierId, updatedSupplierDto);

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            result.Message.Should().Be("Could not update the supplier. Check for duplicate data.");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.GetWithSpecAsync(It.IsAny<SupplierSpecification>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                uow => uow.SupplierRepository.AnyAsync(It.IsAny<Expression<Func<Supplier, bool>>>()),
                Times.Exactly(3));

            _unitOfWorkMock.Verify(
                uow => uow.CompleteAsync(),
                Times.Once);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) =>
                        state.ToString()!.Contains(
                            $"Failed to update supplier {supplierId}")),
                    It.Is<Exception?>(e => ReferenceEquals(e, ex)),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion
    }
}
