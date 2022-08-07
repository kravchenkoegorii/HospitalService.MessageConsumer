namespace HospitalService.MessageConsumer.Models
{
    public class Message
    {
        public Message(string value)
        {
            Value = value;
        }
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
