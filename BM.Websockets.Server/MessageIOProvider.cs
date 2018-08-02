using Microsoft.Extensions.DependencyInjection;
using BM.Common;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace BM.Websockets.Server
{
    public class MessageIOProvider : IMessageIOProvider
    {
        private readonly ConcurrentQueue<Message> messsageQueue;

        public bool HaveMessages => !this.messsageQueue.IsEmpty;
     
        public MessageIOProvider(IServiceProvider serviceProvider)
        {
            this.messsageQueue = new ConcurrentQueue<Message>();
        }

        public async Task Receive(Message message)
        {
            throw new NotImplementedException();
        }

        public void Send(Message message)
        {
            this.messsageQueue.Enqueue(message);
        }

        public Message GetMessage()
        {
            messsageQueue.TryDequeue(out Message message);
            return message;
        }
    }
}
