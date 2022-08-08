using HospitalService.MessageConsumer.Data;
using HospitalService.MessageConsumer.Models;

namespace HospitalService.MessageConsumer.ServiceBusMessaging
{
    public class ProcessData : IProcessData
    {
        private readonly string _connectionString = Environment.GetEnvironmentVariable("MESSAGEDB_KEY");

        public ProcessData() { }

        public async Task Process(string value)
        {
            using (var payloadMessageContext = new PayloadMessageContext(_connectionString))
            {
                await payloadMessageContext.AddAsync(new Message(value));
                await payloadMessageContext.SaveChangesAsync();
            }
        }

    }
}
