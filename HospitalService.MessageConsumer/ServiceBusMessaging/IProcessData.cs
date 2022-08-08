namespace HospitalService.MessageConsumer.ServiceBusMessaging
{
    public interface IProcessData
    {
        Task Process(string value);
    }
}