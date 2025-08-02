
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helper;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
       
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025TpiEntities")); //
        });
        builder.Services.AddSwaggerGen(o => /*CONFIGURACION Y REQUISITOS DE SEGURIDAD PARA QUE EL SWAGGER ADMITA LA AUTENTICACION*/
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TPI PT1- DSW2025",
                Version = "v1",
            });
            o.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Name = "Authorization",
                Description = "Ingrese la palabra 'Bearer' seguido su token. Ejemplo: Bearer ey30xmaMs...",
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
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>  /*Aca se configura todo las propiedades del identity*/
        {
            options.Password = new PasswordOptions
            {
                RequiredLength = 8,

            };
            options.Lockout= new LockoutOptions
            {
                DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5), /*Tiempo de bloqueo por defecto*/
                MaxFailedAccessAttempts = 5, /*Cantidad de intentos fallidos antes de bloquear al usuario*/
                AllowedForNewUsers = true /*Permite que los nuevos usuarios sean bloqueados si superan el maximo de intentos fallidos*/
            };
        })
       .AddEntityFrameworkStores<AuthenticateContext>()
       .AddDefaultTokenProviders();


        var jwtConfig = builder.Configuration.GetSection("Jwt");
        var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("Key is not configured in Jwt settings.");
        var key = Encoding.UTF8.GetBytes(keyText);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  /*Le decimos al servicio de autenticacion los esquemas a usar por defecto*/
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters /*Para metros para configurar los tokens, es recomendable usar las prop como vlidatelifetime aca y no en el app seting*/
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["Issuer"],
                    ValidAudience = jwtConfig["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        builder.Services.AddTransient<JwtTokenService>(); /*Inyeccion de dependencia para el servicio de generacion de tokens, el cual debe ser el mismo*/
  
        builder.Services.AddDbContext<AuthenticateContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025TpiEntities"));
        }); 

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

                var roleMaganer = services.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = new[] { "Admin", "User" };

                foreach(var role in roles)
                {
                    var roleExist = await roleMaganer.RoleExistsAsync(role);
                    if (!roleExist)
                    {
                        await roleMaganer.CreateAsync(new IdentityRole(role));
                    }


                }
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                var adminUserEmail = "admin@example.com";
                var adminUser = await userManager.FindByEmailAsync(adminUserEmail);

                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = "admin",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }


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
        app.UseMiddleware<UserExceptionHandlerMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();
        
        app.MapHealthChecks("/healthcheck");

        await app.RunAsync();
    }
}
