using Pharmacy.System.Core.Dtos.AppUserDtos;
using Pharmacy.System.Core.Models;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Core.Dtos.ProductPackagingDtos;
using Pharmacy.System.Core.Dtos.SupplierDtos;
using System.Linq;
using Pharmacy.System.Core.Dtos.SupplyDtos;
using Pharmacy.System.Core.Dtos.SupplyItemDtos;
using Pharmacy.System.Core.Enums;
using Pharmacy.System.Core.Dtos.ProductCategoryDtos;
using Pharmacy.System.Core.Dtos.SupplyInstallmentDtos;
using Pharmacy.System.Core.Dtos.SupplyPaymentDtos;
using Pharmacy.System.Core.Dtos.InventoryDtos;
using System;
using Pharmacy.System.Core.Dtos.InventoryItemDtos;
using Pharmacy.System.Core.Extensions;

namespace Pharmacy.System.Services.ApplicatoinServicesConfig
{
    public static class MappingProfiles
    {
        public static IServiceCollection AddMappingProfilesConfigurations(
            this IServiceCollection services,
            IConfiguration config)
        {
            Configure(config["Storage:BaseUrl"]);

            return services;
        }

        public static void Configure(string? baseUrl)
        {

            TypeAdapterConfig<AppUser, UserDetailsDto>
                .NewConfig()
                .Map(dest => dest.ImageUrl, src => src.Image == null ? null : $"{baseUrl}/files/UserImages/{src.Image}");


            TypeAdapterConfig<Product, ProductDetailsDto>
                .NewConfig()
                .Map(dest => dest.Image, src => src.Image == null ? null : $"{baseUrl}/files/ProductImages/{src.Image}")
                .Map(dest => dest.ActiveIngredients, src => src.ProductActiveIngredients)
                .Map(dest => dest.Barcodes, src => src.Barcodes.Select(x=>x.Barcode));

            TypeAdapterConfig<ProductDto, Product>
                .NewConfig()
                .Map(dest => dest.Barcodes, src => src.Barcodes.Select(x=> new ProductBarcode()
                {
                    Barcode = x
                }));

            TypeAdapterConfig<UpdateProductDto, Product>
                .NewConfig()
                .Map(dest => dest.Barcodes, src => src.Barcodes.Select(x=> new ProductBarcode()
                {
                    Barcode = x
                }));

            TypeAdapterConfig<ProductCategory, ProductCategoryDetailsDto>
                .NewConfig()
                .Map(dest => dest.Image, src => src.Image == null ? null : $"{baseUrl}/files/CategoryImages/{src.Image}");

            TypeAdapterConfig<ProductActiveIngredient, ProductActiveIngredientDetailsDto>
                .NewConfig()
                .Map(dest => dest.Name, src => src.ActiveIngredient.Name)
                .Map(dest => dest.Id, src => src.ActiveIngredient.Id)
                .Map(dest => dest.Description, src => src.ActiveIngredient.Description);

            TypeAdapterConfig<ProductPackagingLevel, ProductPacagingDetailsDto>
                .NewConfig()
                .Map(dest => dest.PackagingUnitName, src => src.PackagingUnit.Name)
                .Map(dest => dest.ChildPackagingUnitQuantity, src => src.QuantityOfChildPackage);

            TypeAdapterConfig<Supplier, SupplierDetailsDto>
                .NewConfig()
                .Map(dest => dest.Logo, src => src.Logo == null ? null : $"{baseUrl}/files/SupplierImages/{src.Logo}")
                .Map(dest => dest.PhoneNumbers, src => src.PhoneNumbers.Select(x => x.Number));

            TypeAdapterConfig<SupplierDto, Supplier>
                .NewConfig()
                .Map(dest => dest.PhoneNumbers, src => src.PhoneNumbers.Select(x => new SupplierPhoneNumber() { Number = x }));

            TypeAdapterConfig<UpdateSupplierDto, Supplier>
                .NewConfig()
                .Map(dest => dest.PhoneNumbers, src => src.PhoneNumbers.Select(x => new SupplierPhoneNumber() { Number = x }));

            TypeAdapterConfig<SupplyDto, Supply>
                .NewConfig()
                .Ignore(dest => dest.Items)
                .Map(dest => dest.TotalCost, src => src.Items.Sum(x => x.Quantity * x.PurchaseCostPerReceivedUnit))
                .Map(dest => dest.DiscountPercentage, src => src.DiscountPercentage ?? 0)
                .Map(dest => dest.FinalCost, src => src.Items.Sum(x => x.Quantity * x.PurchaseCostPerReceivedUnit)
                                                        * (1 - (src.DiscountPercentage ?? 0) / 100m))
                .Map(dest => dest.ReceivedAt, src => src.ReceivedAt)
                .Map(dest => dest.SupplierId, src => src.SupplierId)
                .Map(dest => dest.PaymentPlan, src => src.PaymentPlan)
                .Map(dest => dest.PaymentStatus, src =>
                src.PaymentPlan == SupplyPaymentPlan.FullPayment ? PaymentStatus.FullyPaid : PaymentStatus.NotPaid);

            TypeAdapterConfig<UpdateSupplyPaymentDto, SupplyPayment>
                .NewConfig()
                .IgnoreNullValues(true);

            TypeAdapterConfig<SupplyItemDto, SupplyItem>
                .NewConfig()
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.ProductBatchId, src => src.ProductBatchId)
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.PackagingLevelId, src => src.PackagingLevelId);

            TypeAdapterConfig<Product, ProductSnapshot>
                .NewConfig()
                .Map(dest => dest.productId, src => src.Id)
                .Map(dest => dest.CategoryName, src => src.Category.Name)
                .Map(dest => dest.ManufacturerName, src => src.Manufacturer.Name)
                .Map(dest => dest.ProductFormName, src => src.ProductForm.Name)
                .Map(dest => dest.ActiveIngredientNames, src => src.ProductActiveIngredients.
                Select(x => x.ActiveIngredient.Name));

            TypeAdapterConfig<ProductPackagingLevel, PackagingLevelSnapshot>
                .NewConfig()
                .Map(dest => dest.LevelId, src => src.Id)
                .Map(dest => dest.UnitName, src => src.PackagingUnit.Name);

            TypeAdapterConfig<Supply, SupplyDetailsDto>
                .NewConfig()
                .Map(dest => dest.SupplierName, src => src.Supplier.Name)
                .Map(dest => dest.Items, src => src.Items);

            TypeAdapterConfig<SupplyItem, SupplyItemDetailsDto>
                .NewConfig()
                .Map(dest => dest.ProductName, src => src.ProductSnapshot.Name)
                .Map(dest => dest.ProductCategoryName, src => src.ProductSnapshot.CategoryName)
                .Map(dest => dest.ProductFormName, src => src.ProductSnapshot.ProductFormName);

            TypeAdapterConfig<SupplyItem, InventoryItem>
                .NewConfig()
                .Ignore(dest=>dest.ProductBatch)
                .Ignore(dest=>dest.Id)
                .Map(dest => dest.ProductId, src => src.ProductId ?? -1)
                .Map(dest => dest.RepositoryId, src => src.Supply.RepositoryId)
                .Map(dest => dest.SupplyItemId, src => src.Id)
                .Map(dest => dest.ProductBatchId, src => src.ProductBatchId)
                .Map(dest => dest.ReceivedBaseUnits, src => src.BaseUnitsPerReceivedUnit * src.Quantity)
                .Map(dest => dest.OnHandBaseUnits, src => src.BaseUnitsPerReceivedUnit * src.Quantity)
                .Map(dest => dest.SellingPricePerBaseUnit, src => src.SellingPricePerReceivedUnit / src.BaseUnitsPerReceivedUnit)
                .Map(dest => dest.PurchaseCostPerBaseUnit, src => src.EffectiveUnitCost / src.BaseUnitsPerReceivedUnit);

            TypeAdapterConfig<SupplyInstallment, SupplyInstallmentDetailsDto>
                .NewConfig()
                .Map(Dest => Dest.Payments, src => src.Payments);

            TypeAdapterConfig<InventoryItem, ProductStockGroupDetails>
                .NewConfig()
                .Map(dest => dest.ProductName, src => src.Product.Name)
                .Map(dest => dest.ProductCategory, src => src.Product.Category.Name)
                .Map(dest => dest.ProductBarcodes, src => src.Product.Barcodes.Select(x => x.Barcode))
                .Map(dest => dest.RepositoryName, src => src.Repository.Name)
                .Map(dest => dest.ReminingDaysToExpiry, src => Math.Max((src.ProductBatch.ExpiryDate.ToDateTime(new TimeOnly()) - DateTime.Today).Days, 0))
                .Map(dest => dest.TotalPurchaseCost, src => src.ReceivedBaseUnits * src.PurchaseCostPerBaseUnit)
                .Map(dest => dest.RemainingQuantitySellingPrice, src => src.OnHandBaseUnits * src.SellingPricePerBaseUnit)
                .Map(dest => dest.SupplierName, src => src.SupplyItem.Supply.Supplier.Name)
                .Map(dest => dest.ExpiryDate, src => src.ProductBatch.ExpiryDate)
                .Map(dest => dest.RemainingQuantity, src => src.CalculateQuantity(true, null))
                .Map(dest => dest.ReceivedQuantity, src => src.CalculateQuantity(false, null));

            TypeAdapterConfig<InventoryItem, InventoryItemDetailsDto>
                .NewConfig()
                .Map(dest => dest.RemainingQuantity, src => src.CalculateQuantity(true, null))
                .Map(dest => dest.ReceivedQuantity, src => src.CalculateQuantity(false, null))
                .Map(dest => dest.RemainingQuantitySellingPrice, src => src.OnHandBaseUnits * src.SellingPricePerBaseUnit)
                .Map(dest => dest.TotalPurchaseCost, src => src.ReceivedBaseUnits * src.PurchaseCostPerBaseUnit);
        }
    }
}
