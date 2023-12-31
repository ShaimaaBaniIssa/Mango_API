﻿using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models.Dto.CartDto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string emailRegisterUserQueue;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedEmail_Subscription;


        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _emailRegisterUserProcessor;
        private ServiceBusProcessor _emailOrderCreated;


        public AzureServiceBusConsumer(IConfiguration configuration , EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            emailRegisterUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailRegisterUserQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            // listening to the queue for any new messages
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _emailRegisterUserProcessor = client.CreateProcessor(emailRegisterUserQueue);

            _emailService = emailService; 
            /////
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedEmail_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedEmail_Subscription");

            _emailOrderCreated = client.CreateProcessor(orderCreatedTopic,orderCreatedEmail_Subscription);


        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

            _emailRegisterUserProcessor.ProcessMessageAsync += OnEmailRegisterUserReceived;
            _emailRegisterUserProcessor.ProcessErrorAsync += ErrorHandler;

            _emailOrderCreated.ProcessMessageAsync += OnNewOrderEmailRequestReceived;
            _emailOrderCreated.ProcessErrorAsync += ErrorHandler;

            await _emailRegisterUserProcessor.StartProcessingAsync();
            await _emailCartProcessor.StartProcessingAsync();
            await _emailOrderCreated.StartProcessingAsync();

        }

        private async Task OnNewOrderEmailRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(body);

            try
            {
                await _emailService.LogOrderPlaced(rewardMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task OnEmailRegisterUserReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            string email = JsonConvert.DeserializeObject<string>(body);

            try
            {
                await _emailService.EmailRegisterUser(email);

                // this message has been processed successfully and removed from queue
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _emailRegisterUserProcessor.StopProcessingAsync() ;
            await _emailRegisterUserProcessor.DisposeAsync();

            await _emailOrderCreated.StopProcessingAsync();
            await _emailOrderCreated.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            // this where will you receive message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
             CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(body);
            try
            {
                await _emailService.EmailCartAndLog(cartDto);
                // this message has been processed successfully and removed from queue
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        
    }
}
