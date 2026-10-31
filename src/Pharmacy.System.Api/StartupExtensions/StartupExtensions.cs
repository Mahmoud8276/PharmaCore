using Pharmacy.System.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Pharmacy.System.Api.StartupExtensions
{
    public static class StartupExtensions
    {
        public static async Task ExecuteStartupExtensions(this WebApplication app)
        {

            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }

            await scope.SeedDataAsync();
        }
    }
}
