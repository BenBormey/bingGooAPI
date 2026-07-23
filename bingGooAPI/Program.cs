using JuJuBiAPI.Configurations;
using JuJuBiAPI.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfoConfiguration(builder.Configuration);
builder.Services.RegisterServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

const string CorsPolicy = "AllowFrontend";

// Production: list the real frontend URLs in config —
//   "Cors": { "AllowedOrigins": [ "https://pos.example.com" ] }
// (or env var Cors__AllowedOrigins__0). With nothing configured the policy
// stays wide open, which is only acceptable for development.
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JuJuBiAPI",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{

    app.UseSwagger(c =>
    {
        c.RouteTemplate = "JuJuBi/swagger/{documentName}/swagger.json";
    });

  
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/JuJuBi/swagger/v1/swagger.json", "JuJuBiAPI v1");
        c.RoutePrefix = "JuJuBi/swagger";
        c.DocumentTitle = "JuJuBi API";
    });
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();