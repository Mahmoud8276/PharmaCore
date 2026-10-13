using Pharmacy.System.Core.Dtos.AppUserDtos;
using Pharmacy.System.Core.Models;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pharmacy.System.Core.Dtos.ProductDtos;
using Pharmacy.System.Core.Dtos.ProductPackagingDtos;
using Pharmacy.System.Core.Dtos.SupplierDtos;
using System.Linq;

namespace Pharmacy.System.Services.ApplicatoinServicesConfig
{
    public static class MappingProfiles
    {
        public static IServiceCollection AddMappingProfilesConfigurations(
            this IServiceCollection services,
            IConfiguration config)
        {
            var BaseUrl = config["Storage:BaseUrl"];


            TypeAdapterConfig<AppUser, UserDetailsDto>
                .NewConfig()
                .Map(dest => dest.ImageUrl, src => src.Image == null? null : $"{BaseUrl}/files/UserImages/{src.Image}");


            TypeAdapterConfig<Product, ProductDetailsDto>
                .NewConfig()
                .Map(dest => dest.Image, src => src.Image == null ? null : $"{BaseUrl}/files/ProductImages/{src.Image}")
                .Map(dest => dest.ActiveIngredients, src => src.ProductActiveIngredients);

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
                .Map(dest => dest.Logo, src => src.Logo == null ? null : $"{BaseUrl}/files/ProductImages/{src.Logo}")
                .Map(dest => dest.PhoneNumbers, src => src.PhoneNumbers.Select(x=>x.Number));

            TypeAdapterConfig<SupplierDto, Supplier>
                .NewConfig()
                .Map(dest => dest.PhoneNumbers, src => src.PhoneNumbers.Select(x=>new SupplierPhoneNumber() { Number = x}));

            TypeAdapterConfig<UpdateSupplierDto, Supplier>
                .NewConfig()
                .Map(dest => dest.PhoneNumbers, src => src.PhoneNumbers.Select(x=>new SupplierPhoneNumber() { Number = x}));

            return services;
        }
    }
}
