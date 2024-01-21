
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.Dto.CartDto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Channels;

namespace Mango.Services.EmailAPI.Messaging
{
    public class RabbitMQOrderConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private IConnection _connection;
        private IChannel _channel;
        private readonly string orderCreatedExchange;
        string queueName="";
        private const string orderCreated_EmailQueue = "EmailQueue";



        public RabbitMQOrderConsumer(IConfiguration configuration , EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            orderCreatedExchange = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateChannel();
            _channel.ExchangeDeclare(orderCreatedExchange,type:ExchangeType.Direct);

            _channel.QueueDeclare(orderCreated_EmailQueue, false, false, false, null); // automatically declare queue for the exchange
            _channel.QueueBind(orderCreated_EmailQueue, orderCreatedExchange, "EmailUpdate");

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(content);
                HandleMessage(rewardMessage).GetAwaiter().GetResult();

                // Acknowledgment
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            _channel.BasicConsume(orderCreated_EmailQueue, false, consumer);
            return Task.CompletedTask;
        }
        private async Task HandleMessage(RewardMessage rewardMessage)
        {
            _emailService.LogOrderPlaced(rewardMessage).GetAwaiter().GetResult();
        }
    }
}
