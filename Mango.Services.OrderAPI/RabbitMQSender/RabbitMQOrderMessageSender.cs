using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.OrderAPI.RabbitMQSender
{
    public class RabbitMQOrderMessageSender :IRabbitMQOrderMessageSender
    {
        // to setup a connection to RabbitMQ
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private IConnection _connection;
        private const string orderCreated_RewardsQueue = "RewardsQueue";
        private const string orderCreated_EmailQueue = "EmailQueue";

        public RabbitMQOrderMessageSender()
        {
            _hostName = "localhost";
            _userName = "guest";
            _password = "guest";

        }
        public void SendMessage(object msg, string exchangeName)
        {
            ConnectionExist();
            // establish channel to communicate
            using var channel = _connection.CreateChannel();
            channel.ExchangeDeclare(exchangeName, type: ExchangeType.Direct, durable: false);
           
            channel.QueueDeclare(orderCreated_EmailQueue, false, false, false, null);
            channel.QueueDeclare(orderCreated_RewardsQueue, false, false, false, null);

            channel.QueueBind(orderCreated_EmailQueue, exchangeName,"EmailUpdate");
            channel.QueueBind(orderCreated_RewardsQueue, exchangeName,"RewardsUpdate"); //  RewardsUpdate routing key

            var json = JsonConvert.SerializeObject(msg);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: exchangeName, "EmailUpdate", body: body);
            channel.BasicPublish(exchange: exchangeName, "RewardsUpdate", body: body);

        }
        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password
                };
                // establish the connection
                _connection = factory.CreateConnection();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void ConnectionExist()
        {
            if (_connection == null)
                CreateConnection();
            
        }
    }
}
