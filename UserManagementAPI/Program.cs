using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Repositories;
using UserManagementAPI.Services;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Use Microsoft.AspNetCore.OpenApi package helpers (AddOpenApi / MapOpenApi)
builder.Services.AddOpenApi();

// Configure CORS - allow local development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});

// DbContext - InMemory for now
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseInMemoryDatabase("UserDb"));

// DI for repository and services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");

// Global error handling middleware (should be early in the pipeline)
app.UseErrorHandling();

// Token authentication (validates Authorization: Bearer <token>)
app.UseTokenAuthentication();

// Request/Response logging
app.UseRequestResponseLogging();

app.MapControllers();

app.Run();
