using CtiHub.Application.Common.Interfaces;
using CtiHub.Infrastructure.Persistence;   // DbContext için
using CtiHub.Infrastructure.Repositories;  // Repositories için
using CtiHub.Infrastructure.Services;      // RabbitMQService için
using Microsoft.EntityFrameworkCore;       // EF Core için
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CtiHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Veritabanı Bağlantısı (Program.cs'ten buraya taşıdık)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // 2. Repository Tanımları (Program.cs'ten buraya taşıdık)
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // 3. RabbitMQ Servisi
        services.AddScoped<IRabbitMqService, RabbitMqService>();

        return services;
    }
}