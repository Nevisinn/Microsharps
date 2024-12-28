using System.Text;
using System.Text.Json;
using AbstractTaskService.Logic.Models;
using AbstractTaskService.Logic.Requests;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AbstractTaskService.Logic;

public class MessageSender
{
    private readonly ConnectionFactory factory;

    public MessageSender()
    {
        factory = new ConnectionFactory { HostName = "localhost" };
    }

    public async Task SendMessage(AbstractTask message)
    {
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(queue: "hello", durable: false, exclusive: false, autoDelete: false,
            arguments: null);
        
        var messageBody = await SerializeToByteAsync(message);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: messageBody);
    }
    
    private async Task<byte[]> SerializeToByteAsync(AbstractTask obj)
    {
        using var memoryStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memoryStream, obj);
        return memoryStream.ToArray();
    }
}