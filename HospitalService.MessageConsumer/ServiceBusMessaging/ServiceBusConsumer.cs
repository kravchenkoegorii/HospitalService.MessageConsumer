using Azure.Messaging.ServiceBus;

namespace HospitalService.MessageConsumer.ServiceBusMessaging
{
    public class ServiceBusConsumer : IServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IProcessData _processData;
        private readonly ServiceBusClient _client;
        private ServiceBusProcessor _processor;
        private const string QUEUE_NAME = "message-queue";


        public ServiceBusConsumer(IConfiguration configuration, ILogger<ServiceBusConsumer> logger, IProcessData processData)
        {
            _configuration = configuration;
            _logger = logger;
            _processData = processData;

            var connectionString = Environment.GetEnvironmentVariable("HOSPITALBUS_KEY");
            _client = new ServiceBusClient(connectionString);
        }

        public async Task RegisterOnMessageHandlerAndReceiveMessages()
        {
            ServiceBusProcessorOptions _serviceBusProcessorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };

            _processor = _client.CreateProcessor(QUEUE_NAME, _serviceBusProcessorOptions);
            _processor.ProcessMessageAsync += ProcessMessagesAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;
            await _processor.StartProcessingAsync();
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Message handler encountered an exception");
            _logger.LogDebug($"- ErrorSource: {arg.ErrorSource}");
            _logger.LogDebug($"- Entity Path: {arg.EntityPath}");
            _logger.LogDebug($"- FullyQualifiedNamespace: {arg.FullyQualifiedNamespace}");

            return Task.CompletedTask;
        }

        private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
        {
            var msj = args.Message;
            var myData = msj.Body.ToString();
            Console.WriteLine(myData);
            await _processData.Process(myData);
            await args.CompleteMessageAsync(msj);
        }

        public async ValueTask DisposeAsync()
        {
            if (_processor != null)
            {
                await _processor.DisposeAsync().ConfigureAwait(false);
            }

            if (_client != null)
            {
                await _client.DisposeAsync().ConfigureAwait(false);
            }
        }

        public async Task CloseQueueAsync()
        {
            await _processor.CloseAsync().ConfigureAwait(false);
        }
    }
}
