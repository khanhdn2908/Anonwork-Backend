using Anonwork.Application.Interfaces;
using Anonwork.Application.Services;
using Anonwork.Domain.Repositories;
using Anonwork.Infrastructure;
using Anonwork.Infrastructure.Repositories;
using Microsoft.OpenApi;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
builder.Configuration.AddEnvironmentVariables();

var enableSwagger = Environment.GetEnvironmentVariable("EnableSwagger");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add Services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Anonwork API",
        Version = "v1"
    });
});

// Add Infrastructure
try
{
    builder.Services.AddInfrastructure(builder.Configuration);
}
catch (ReflectionTypeLoadException ex)
{
    foreach (var loaderException in ex.LoaderExceptions)
    {
        Console.WriteLine(loaderException.Message);
    }

    throw;
}

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure HTTP Pipeline

if (app.Environment.IsDevelopment() || enableSwagger == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();