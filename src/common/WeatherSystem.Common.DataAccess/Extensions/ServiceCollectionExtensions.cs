using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherSystem.Common.DataAccess.Repositories;

namespace WeatherSystem.Common.DataAccess.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services
                .AddConnectionStringConfig(configuration)
                .AddConnectionFactory();
        }

        private static IServiceCollection AddConnectionFactory(this IServiceCollection services)
        {
            return services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        }

        private static IServiceCollection AddConnectionStringConfig(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DbConfiguration>(configuration.GetSection("ConnectionStrings").Bind);
            return services;
        }
    }
}