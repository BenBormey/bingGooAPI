using bingGooAPI.Databases;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Services;
using Microsoft.Data.SqlClient;
using System.Data;
//using bingGooAPI.Services();
namespace bingGooAPI.Configurations
{
    public static class ServicesConfigurations
    {
        public static void RegisterServices(this IServiceCollection services)
        {

            services.AddScoped<ICurrency, CurrencyService>();
            services.AddScoped<ICategory, CategoryService>();
            services.AddScoped<Ibrand, BranchService>();
 services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOutletRepository, OutletService>();


        }

        public static IServiceCollection AddInfoConfiguration(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            var maxRetryCount = 5;
            var maxRetryDelay = 60;

           var defaultConnection = configuration.GetConnectionString("DefaultConnection");
            services.AddScoped<IDbConnection>(sp =>
            {
                var cs = configuration.GetConnectionString("DefaultConnection");
                return new SqlConnection(cs);
            });
            //var untServerConnection = configuration.GetConnectionString("UntServerConnection");

            services.AddSingleton<ConnectionManager>(sp =>
                new ConnectionManager(defaultConnection));
            return services;
        }
    }
}
