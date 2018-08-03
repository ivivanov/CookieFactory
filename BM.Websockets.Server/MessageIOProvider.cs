using BM.Common;
using System;
using System.Collections.Concurrent;

namespace BM.Websockets.Server
{
    public class MessageIOProvider : IMessageIOProvider
    {
        private readonly ConcurrentQueue<string> messsageQueue;

        public bool HaveMessages => !this.messsageQueue.IsEmpty;
     
        public MessageIOProvider(IServiceProvider serviceProvider)
        {
            this.messsageQueue = new ConcurrentQueue<string>();
        }

        public void Receive(string message)
        {
            throw new NotImplementedException();
        }

        public void Send(string message)
        {
            this.messsageQueue.Enqueue(message);
        }

        public string GetMessage()
        {
            messsageQueue.TryDequeue(out string message);
            return message;
        }
    }
}
