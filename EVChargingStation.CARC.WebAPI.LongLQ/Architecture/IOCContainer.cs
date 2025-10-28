using EVChargingStation.CARC.Application.LongLQ.Interfaces;
using EVChargingStation.CARC.Application.LongLQ.Services;
using EVChargingStation.CARC.Domain.LongLQ;
using EVChargingStation.CARC.Infrastructure.Commons;
using EVChargingStation.CARC.Infrastructure.LongLQ;
using EVChargingStation.CARC.Infrastructure.LongLQ.Commons;
using EVChargingStation.CARC.Infrastructure.LongLQ.Interfaces;
using EVChargingStation.CARC.Infrastructure.LongLQ.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace EVChargingStation.CARC.WebAPI.LongLQ.Architecture;

public static class IocContainer
{
    public static IServiceCollection SetupIocContainer(this IServiceCollection services)
    {

        //Add Project Services
        services.SetupDbContext();
        services.SetupSwagger();

        //Add generic repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        //Add business services
        services.SetupBusinessServicesLayer();

        services.SetupJwt();

        return services;
    }

    private static IServiceCollection SetupDbContext(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        // Lấy connection string từ "DefaultConnection"
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Đăng ký DbContext với Npgsql - Postgres
        services.AddDbContext<FA25_SWD392_SE182594_G6_EvChargingStation>(options =>
            options.UseNpgsql(connectionString,
                sql => sql.MigrationsAssembly(typeof(FA25_SWD392_SE182594_G6_EvChargingStation).Assembly.FullName)
            )
        );

        return services;
    }

    public static IServiceCollection SetupBusinessServicesLayer(this IServiceCollection services)
    {
        services.AddScoped<ICurrentTime, CurrentTime>();
        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IStationService, StationService>();

        services.AddHttpContextAccessor();

        return services;
    }

    private static IServiceCollection SetupSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.UseInlineDefinitionsForEnums();

            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "EVChargingStationAPI",
                Version = "v1",
                Description = "API for EV Charging Station Management",
            });
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter your JWT token in this field",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
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
                    new string[] { }
                }
            };

            c.AddSecurityRequirement(securityRequirement);

            // Cấu hình Swagger để sử dụng Newtonsoft.Json
            c.UseAllOfForInheritance();

            c.EnableAnnotations();
        });

        return services;
    }

    private static IServiceCollection SetupJwt(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,   // Bật kiểm tra Issuer
                    ValidateAudience = true, // Bật kiểm tra Audience
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"] ??
                                                                        throw new InvalidOperationException()))
                };
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("DriverPolicy", policy =>
                policy.RequireRole("Driver"));

            options.AddPolicy("StaffPolicy", policy =>
                policy.RequireRole("Staff"));

            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireRole("Admin"));
        });

        return services;
    }
}