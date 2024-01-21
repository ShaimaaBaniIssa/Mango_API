using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.ShoppingCartAPI.RabbitMQSender
{
    public class RabbitMQCartMessageSender :IRabbitMQCartMessageSender
    {
        // to setup a connection to RabbitMQ
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private IConnection _connection;

        public RabbitMQCartMessageSender()
        {
            _hostName = "localhost";
            _userName = "guest";
            _password = "guest";

        }
        public void SendMessage(object msg, string queueName)
        {
            ConnectionExist();
            // establish channel to communicate
            using var channel = _connection.CreateChannel();
            channel.QueueDeclare(queueName, false, false, false, null);
            var json = JsonConvert.SerializeObject(msg);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "", routingKey: queueName,body: body);
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
