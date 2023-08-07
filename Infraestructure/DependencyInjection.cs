using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data.LocalDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // services.AddSingleton<IDataHandler<User>, MemoryDataManager>();
            services.AddSingleton<IDataHandler<User>, LocalDbDataManager>();

            return services;
        }
    }
}