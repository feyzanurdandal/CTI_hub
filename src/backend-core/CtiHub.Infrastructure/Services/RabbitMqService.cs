using System.Text;
using System.Text.Json;
using CtiHub.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client; // v7.0.0+

namespace CtiHub.Infrastructure.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly IConfiguration _configuration;

    public RabbitMqService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        // 1. Ayarları Oku
        var hostName = _configuration["RabbitMqSettings:HostName"] ?? "localhost";
        var userName = _configuration["RabbitMqSettings:UserName"] ?? "guest";
        var password = _configuration["RabbitMqSettings:Password"] ?? "guest";

        // 2. Bağlantı Fabrikasını Kur
        var factory = new ConnectionFactory()
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };

        // 3. Bağlantıyı Aç (ASYNC oldu)
        // 'using' yerine 'await using' kullanıyoruz çünkü bağlantı işi bitince asenkron kapanmalı.
        await using var connection = await factory.CreateConnectionAsync();
        
        // 4. Kanal Oluştur (Eskiden CreateModel idi, şimdi CreateChannelAsync oldu)
        await using var channel = await connection.CreateChannelAsync();

        // 5. Kuyruğu Tanımla (ASYNC oldu)
        await channel.QueueDeclareAsync(queue: queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        // 6. Mesajı Hazırla
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        // 7. Mesajı Gönder (ASYNC oldu)
        // Exchange boş string ("") ise default exchange kullanılır.
        await channel.BasicPublishAsync(exchange: "", 
                                        routingKey: queueName, 
                                        mandatory: false, 
                                        body: body);
    }
}