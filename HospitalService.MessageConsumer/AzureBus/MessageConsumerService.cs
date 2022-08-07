using Azure.Messaging.ServiceBus;
using HospitalService.MessageConsumer.Data;
using HospitalService.MessageConsumer.Models;

namespace HospitalService.MessageConsumer.AzureBus
{
    public class MessageConsumerService : BackgroundService, IHostedService
    {
        private readonly MessageDbContext _dbContext;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusReceiver _serviceBusReceiver;

        public MessageConsumerService(MessageDbContext dbContext)
        {
            _dbContext = dbContext;
            _serviceBusClient = new ServiceBusClient(Environment.GetEnvironmentVariable("HOSPITALBUS_KEY"));
            _serviceBusReceiver = _serviceBusClient.CreateReceiver("message-queue");
        }

        private async Task MessageHandler(CancellationToken cancellationToken)
        {
            while(true)
            {
                    ServiceBusReceivedMessage serviceBusReceivedMessage = await _serviceBusReceiver.ReceiveMessageAsync();
                    var body = serviceBusReceivedMessage.Body.ToString();
                    await _dbContext.Messages.AddAsync(new Message(body));
                    await _dbContext.SaveChangesAsync();
                    Console.WriteLine(body);
                    await _serviceBusReceiver.CompleteMessageAsync(serviceBusReceivedMessage);      
            }    
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"{nameof(MessageConsumerService)} has started.");
            await MessageHandler(stoppingToken);
        }
    }
}
