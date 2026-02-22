using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CtiHub.Infrastructure.BackgroundJobs;

// BackgroundService'den miras alıyoruz ki API çalışırken bu da arkada hep çalışsın
public class ScanQueueConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ScanQueueConsumer> _logger;

    public ScanQueueConsumer(IConfiguration configuration, ILogger<ScanQueueConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
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

        // 1. Bağlantıyı ve Kanalı Kur (Asenkron)
        var connection = await factory.CreateConnectionAsync(stoppingToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // 2. Hangi kuyruğu dinleyeceğimizi söylüyoruz
        await channel.QueueDeclareAsync(queue: "scan_queue", durable: false, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

        // 3. Tüketiciyi (Consumer) oluşturuyoruz
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        // 4. Mesaj Geldiğinde Ne Olacak? (Tetiklenecek Olay)
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            // Şimdilik sadece konsola yazdırıyoruz. (İleride burada asıl tarama kodunu çağıracağız)
            _logger.LogInformation($"\n\n ***YAKALANDI! RabbitMQ'dan Gelen Görev: {message}\n");
            
            await Task.CompletedTask;
        };

        // 5. Dinlemeyi Başlat! (autoAck: true -> Mesajı alır almaz kuyruktan sil)
        await channel.BasicConsumeAsync(queue: "scan_queue", autoAck: true, consumer: consumer, cancellationToken: stoppingToken);
        
        // Servis kapanana kadar bu döngüyü hayatta tut
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}