using BM.Common;
using System;
using System.Collections.Concurrent;

namespace BM.Websockets.Server
{
    public class MessageIOProvider : IMessageIOProvider
    {
        private readonly ConcurrentQueue<string> outgoingMesssageQueue;
        private readonly ConcurrentQueue<string> incomingMesssageQueue;

        public bool HaveOutgoingMessages => !this.outgoingMesssageQueue.IsEmpty;

        public bool HaveIncomingMessages => !this.incomingMesssageQueue.IsEmpty;
     
        public MessageIOProvider()
        {
            this.outgoingMesssageQueue = new ConcurrentQueue<string>();
            this.incomingMesssageQueue = new ConcurrentQueue<string>();
        }

        public void Receive(string message)
        {
            this.incomingMesssageQueue.Enqueue(message);
        }

        public void Send(string message)
        {
            this.outgoingMesssageQueue.Enqueue(message);
        }

        public string GetOutgoingMessage()
        {
            outgoingMesssageQueue.TryDequeue(out string message);
            return message;
        }

        public string GetIncomingMessage()
        {
            incomingMesssageQueue.TryDequeue(out string message);
            return message;
        }
    }
}
