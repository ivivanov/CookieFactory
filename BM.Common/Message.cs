namespace BM.Common
{
    public class Message
    {
        public Message(MessageType messageType, string data)
        {
            this.MessageType = messageType;
            this.Data = data;
        }

        public MessageType MessageType { get; set; }

        public string Data { get; set; }
    }
}
