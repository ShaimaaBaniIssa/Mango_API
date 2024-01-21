
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Channels;

namespace Mango.Services.EmailAPI.Messaging
{
    public class RabbitMQAuthConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private IConnection _connection;
        private IChannel _channel;
        private readonly string emailRegisterUserQueue;


        public RabbitMQAuthConsumer(IConfiguration configuration , EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            emailRegisterUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailRegisterUserQueue");

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateChannel();
            _channel.QueueDeclare(emailRegisterUserQueue, false, false, false, null);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                String email = JsonConvert.DeserializeObject<String>(content);
                HandleMessage(email).GetAwaiter().GetResult();

                // Acknowledgment
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume(emailRegisterUserQueue, false, consumer);
            return Task.CompletedTask;
        }
        private async Task HandleMessage(string email)
        {
            _emailService.EmailRegisterUser(email).GetAwaiter().GetResult();
        }
    }
}
