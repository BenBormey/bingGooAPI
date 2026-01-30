using bingGooAPI.Configurations;
using bingGooAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ================= SERVICES =================

// DB + Repositories
builder.Services.AddInfoConfiguration(builder.Configuration);
builder.Services.RegisterServices();

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Authorization
builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ================= APP =================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global Exception
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

// ? VERY IMPORTANT ORDER
app.UseAuthentication();   // FIRST
app.UseAuthorization();    // SECOND

app.MapControllers();

app.Run();
