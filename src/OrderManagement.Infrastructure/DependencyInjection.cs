using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Domain.Interfaces.Repositories;
using OrderManagement.Domain.Interfaces.Services;
using OrderManagement.Infrastructure.Caching;
using OrderManagement.Infrastructure.Messaging;
using OrderManagement.Infrastructure.Messaging.Consumers;
using OrderManagement.Infrastructure.MongoDB;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Context;

namespace OrderManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===== SQL Server + Entity Framework =====
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServer"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // ===== Unit of Work =====
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ===== MongoDB =====
        services.Configure<MongoDbSettings>(options =>
        {
            options.ConnectionString = configuration.GetConnectionString("MongoDb")!;
            options.DatabaseName = configuration["MongoDbSettings:DatabaseName"]!;
        });

        services.AddSingleton<IProductRepository, ProductRepository>();

        // ===== Redis Cache =====
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "OrderManagement:";
        });

        services.AddSingleton<ICacheService, RedisCacheService>();

        // ===== RabbitMQ + MassTransit =====
        var rabbitHost = configuration["RabbitMqSettings:Host"] ?? "localhost";
        var rabbitUser = configuration["RabbitMqSettings:Username"] ?? "admin";
        var rabbitPass = configuration["RabbitMqSettings:Password"] ?? "admin";

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<OrderStatusChangedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitHost, "/", h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventPublisher, EventPublisher>();

        // Licença gratuita - fora do UsingRabbitMq
        services.AddOptions<MassTransitHostOptions>()
            .Configure(options => options.WaitUntilStarted = false);


        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}

