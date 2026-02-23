using System.Text;
using System.Text.Json;
using CtiHub.Domain.Entities;
using CtiHub.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CtiHub.Infrastructure.BackgroundJobs;

public class ScanQueueConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ScanQueueConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory; // Veritabanı bağlantısı üretmek için fabrika

    public ScanQueueConsumer(IConfiguration configuration, ILogger<ScanQueueConsumer> logger, IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var hostName = _configuration["RabbitMqSettings:HostName"] ?? "localhost";
        var userName = _configuration["RabbitMqSettings:UserName"] ?? "guest";
        var password = _configuration["RabbitMqSettings:Password"] ?? "guest";

        var factory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };

        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(queue: "scan_queue", durable: false, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);
            
            _logger.LogInformation($"\n🎯 YAKALANDI! Gelen Görev: {messageString}");

            try
            {
                // 1. JSON mesajını parçala ve URL'i al
                var jsonDoc = JsonDocument.Parse(messageString);
                var targetUrl = jsonDoc.RootElement.GetProperty("Url").GetString();

                if (!string.IsNullOrEmpty(targetUrl))
                {
                    // 2. Veritabanına bağlanmak için yeni bir "Scope" (Çalışma Alanı) oluştur
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // 3. Veritabanına "Taramaya Başladım" kaydını at
                        var scanRecord = new ScanRecord
                        {
                            TargetUrl = targetUrl,
                            Status = "InProgress", // Durum: İşleniyor
                            RequestedAt = DateTime.UtcNow
                        };

                        dbContext.ScanRecords.Add(scanRecord);
                        await dbContext.SaveChangesAsync(); // Kaydet
                        _logger.LogInformation($"⏳ Veritabanına kaydedildi: {targetUrl} - Durum: InProgress");

                        // 4. SİMÜLASYON: Gerçek tarama işlemi burada olacak. (Şimdilik 5 saniye bekletiyoruz)
                        _logger.LogInformation($"🔍 {targetUrl} taranıyor... Lütfen bekleyin...");
                        await Task.Delay(5000, stoppingToken);

                        // 5. Tarama bitti, durumu güncelle
                        scanRecord.Status = "Completed"; // Durum: Tamamlandı
                        scanRecord.CompletedAt = DateTime.UtcNow;
                        scanRecord.ResultData = "{ \"info\": \"Sistem güvenli. Açık port bulunamadı.\" }"; // Sahte sonuç

                        dbContext.ScanRecords.Update(scanRecord);
                        await dbContext.SaveChangesAsync(); // Güncellemeyi kaydet
                        _logger.LogInformation($"✅ Tarama tamamlandı ve güncellendi: {targetUrl}\n");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ İşlem sırasında hata: {ex.Message}");
            }

            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue: "scan_queue", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}