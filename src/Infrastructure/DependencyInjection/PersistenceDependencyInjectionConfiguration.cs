using Core.Domain.Orders;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Infrastructure.DependencyInjection
{
    public static class PersistenceDependencyInjectionConfiguration
    {
        public static IServiceCollection AddInfrastructureRepositories(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var config = configuration
                .GetSection("postgresql")
                .Get<PostgreSqlOptions>((a) => a.BindNonPublicProperties = true);

            services
                .AddScoped<IOrderRepository, OrderRepository>()
                .AddEntityFrameworkNpgsql()
                .AddDbContext<DataBaseContext>(opt =>
                    {
                        opt.UseNpgsql(config.ConnectionString);
                    })
            ;

            var dataBaseContext = services
                .BuildServiceProvider()
                .GetRequiredService<DataBaseContext>();

            dataBaseContext.Database.EnsureCreated();

            return services;
        }
    }
}
