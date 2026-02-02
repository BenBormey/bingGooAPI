using bingGooAPI.Databases;
using bingGooAPI.Entities;
using bingGooAPI.Interfaces;
using bingGooAPI.Middlewares;
using bingGooAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

namespace bingGooAPI.Configurations
{
    public static class ServicesConfigurations
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrency, CurrencyService>();
            services.AddScoped<ICategory, CategoryService>();
            services.AddScoped<IbrandRepository, BranchService>();
            services.AddScoped<IProductStockRepository, ProductStockService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IOutletRepository, OutletRepository>();
        }

        // ================= JWT CONFIG =================
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(key)
                            )
                    };
            });

            return services;
        }

        // ================= DB CONFIG =================
        public static IServiceCollection AddInfoConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var defaultConnection =
                configuration.GetConnectionString("DefaultConnection");

            services.AddScoped<IDbConnection>(sp =>
            {
                return new SqlConnection(defaultConnection);
            });

            services.AddSingleton<ConnectionManager>(sp =>
                new ConnectionManager(defaultConnection));

            return services;
        }
    }
}
