
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helper;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
        {
            options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Dsw2025db;Integrated Security=True;"); //
        });
        builder.Services.AddSwaggerGen(o =>
        {
        o.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Desarrollo de Software",
            Version = "v1",
        });
        o.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Name = "Authorization",
            Description = "Ingrese el Token",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
        });
        o.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        
        {
            {
            new OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
                Array.Empty<string>()
             }

        });

        });
        builder.Services.AddHealthChecks();
        builder.Services.AddAuthentication()
            .AddJwtBearer();
        builder.Services.AddScoped<IRepository, EfRepository>();
        builder.Services.AddTransient<ProductManagerService>();
        builder.Services.AddTransient<OrderManagerService>();

        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<Dsw2025TpiContext>();
                context.Database.Migrate();
                context.Seedwork<Customer>("Sources/Customers.json");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error al creando la BD.");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

            app.UseHttpsRedirection();
        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/healthcheck");

        app.Run();
    }
}
