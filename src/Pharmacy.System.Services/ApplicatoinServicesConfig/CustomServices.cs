using Pharmacy.System.Services.IServices;
using Pharmacy.System.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pharmacy.System.Services.ApplicatoinServicesConfig
{
    public static class CustomServices
    {
        public static IServiceCollection AddCustomServicesConfigurations(this IServiceCollection services)
        {

             services.AddScoped<ITokenService, TokenService>();
             services.AddScoped<IAccountService, AccountService>();
             services.AddScoped<IEmailService, EmailService>();
             services.AddScoped<IUserService, UserService>();

             services.AddScoped<IProductCategoryService, ProductCategoryService>();
             services.AddScoped<IProductService, ProductService>();
             services.AddScoped<IProductFormService, ProductFormService>();
             services.AddScoped<IActiveIngredientService, ActiveIngredientService>();
             services.AddScoped<IPackagingUnitService, PackagingUnitService>();
             services.AddScoped<IProductPackagingService, ProductPackagingService>();
             services.AddScoped<ISupplierService, SupplierService>();
             services.AddScoped<ISupplyService, SupplyService>();
             services.AddScoped<ISupplyPaymentService, SupplyPaymentService>();
             services.AddScoped<IRepositoryService, RepositoryService>();

            return services;
        }
    }
}
