using Pharmacy.System.Core.Interfaces;
using Pharmacy.System.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Pharmacy.System.Core.Interfaces.IRepositories;
using Pharmacy.System.Core.Models;

namespace Pharmacy.System.Services.ApplicatoinServicesConfig
{
    public static class Repositories
    {
        public static IServiceCollection AddRepositoriesConfigurations(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IGenericRepository<ProductForm, int>, GenericRepository<ProductForm, int>>();
            services.AddScoped<IGenericRepository<ActiveIngredient, int>, GenericRepository<ActiveIngredient, int>>();
            services.AddScoped<IGenericRepository<PackagingUnit, int>, GenericRepository<PackagingUnit, int>>();
            services.AddScoped<IGenericRepository<Repository, int>, GenericRepository<Repository, int>>();

            return services;
        }
    }
}
