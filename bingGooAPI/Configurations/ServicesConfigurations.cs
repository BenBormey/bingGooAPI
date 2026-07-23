using JuJuBiAPI.Entities;
using JuJuBiAPI.Interfaces;
using JuJuBiAPI.Middlewares;
using JuJuBiAPI.Repositories;
using JuJuBiAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

namespace JuJuBiAPI.Configurations
{
    public static class ServicesConfigurations
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyRepository, CurrencyService>();
            services.AddScoped<ICategoryRepository, CategoryService>();
            services.AddScoped<IBrandRepository, BranchService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOutletRepository, OutletRepository>();
            services.AddScoped<IOutletCodeRepository, OutletCodeRepository>();
            services.AddScoped<ITermDayRepository, TermDayRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IExchangeRateRepository, ExchangeRateService>();
            services.AddScoped<IProvincesRepository, ProvinceRepository>();
            services.AddScoped<IFranchiseRepository, FranchiseRepository>();
            services.AddScoped<IFranchiseTypeItemRepository, FranchiseTypeRepository>();
            services.AddScoped<IProductScalRepository, ProductScalRepository>();
            services.AddScoped<IHourOperationRepository, HourOperationRepository>();
            services.AddScoped<IBankSetupRepository, BankSetupRepository>();
            services.AddScoped<ISupplierReportRepository, SupplierReportService>();
            services.AddScoped<IMenuItemRepository, MenuItemRepository>();
            services.AddScoped<IVatSettingRepository, VatSettingRepository>();
            services.AddScoped<IShiftRepository, ShiftRepository>();
            services.AddScoped<IUomRepository, UomRepository>();
            services.AddScoped<IOutletProductRepository, OutletProductService>();
            services.AddScoped<IShelfLifeRepository, ShelfLifeRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IOutletOrderRepository, OutletOrderRepository>();
            services.AddScoped<IProductDeliveryLogisticRepository, ProductDeliveryLogisticRepository>();
            services.AddScoped<ITransferOrderRepository, TransferOrderRepository>();
        }


        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "Jwt:Key is not configured. In production set the Jwt__Key environment variable.");

            // JwtTokenService stamps these onto every token it issues, so
            // reject any token that was signed for a different app.
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

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
                        ValidateIssuer = !string.IsNullOrEmpty(issuer),
                        ValidIssuer = issuer,
                        ValidateAudience = !string.IsNullOrEmpty(audience),
                        ValidAudience = audience,
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

        public static IServiceCollection AddInfoConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var defaultConnection =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            services.AddScoped<IDbConnection>(sp =>
            {
                return new SqlConnection(defaultConnection);
            });

            return services;
        }
    }
}
