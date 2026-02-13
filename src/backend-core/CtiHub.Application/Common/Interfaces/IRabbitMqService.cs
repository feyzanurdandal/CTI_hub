using System.Threading.Tasks; // Bunu eklemeyi unutma

namespace CtiHub.Application.Common.Interfaces;

public interface IRabbitMqService
{
    // void yerine Task yaptık, metodun sonuna "Async" ekledik
    Task SendMessageAsync<T>(T message, string queueName);
}