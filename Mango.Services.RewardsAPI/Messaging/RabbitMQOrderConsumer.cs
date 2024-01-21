

using Mango.Services.RewardsAPI.Message;
using Mango.Services.RewardsAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Channels;

namespace Mango.Services.RewardsAPI.Messaging
{
    public class RabbitMQOrderConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;
        private IConnection _connection;
        private IChannel _channel;
        private readonly string orderCreatedExchange;
        string queueName = "";
        private const string orderCreated_RewardsQueue = "RewardsQueue";
        


        public RabbitMQOrderConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            _rewardService = rewardService;
            orderCreatedExchange = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateChannel();
            _channel.ExchangeDeclare(orderCreatedExchange, type: ExchangeType.Direct);
            // queueName = _channel.QueueDeclare().QueueName; // default queue automatically declare queue for the exchange

            _channel.QueueDeclare(orderCreated_RewardsQueue, false, false, false, null);
            _channel.QueueBind(orderCreated_RewardsQueue, orderCreatedExchange, "RewardsUpdate");

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
            _channel.BasicConsume(orderCreated_RewardsQueue, false, consumer);
            return Task.CompletedTask;
        }
        private async Task HandleMessage(RewardMessage rewardMessage)
        {
            _rewardService.UpdateRewards(rewardMessage).GetAwaiter().GetResult();
        }
    }
}
