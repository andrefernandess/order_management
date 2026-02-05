using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace OrderManagement.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "OrderManagement API",
                Version = "v1",
                Description = "API para gerenciamento de pedidos",
                Contact = new OpenApiContact
                {
                    Name = "Andre",
                    Email = "andre@email.com"
                }
            });

            // Configuração do JWT no Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o token JWT: Bearer {seu_token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    public static IServiceCollection AddHealthChecksConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitHost = configuration["RabbitMqSettings:Host"] ?? "localhost";
        var rabbitUser = configuration["RabbitMqSettings:Username"] ?? "admin";
        var rabbitPass = configuration["RabbitMqSettings:Password"] ?? "SuaSenha@123";

        services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("SqlServer")!,
                name: "sqlserver",
                tags: new[] { "db", "sql" })
            .AddMongoDb(
                configuration.GetConnectionString("MongoDb")!,
                name: "mongodb",
                tags: new[] { "db", "nosql" })
            .AddRedis(
                configuration.GetConnectionString("Redis")!,
                name: "redis",
                tags: new[] { "cache" })
            .AddRabbitMQ(
                new Uri($"amqp://{rabbitUser}:{Uri.EscapeDataString(rabbitPass)}@{rabbitHost}:5672"),
                name: "rabbitmq",
                tags: new[] { "messaging" });

        return services;
    }
}