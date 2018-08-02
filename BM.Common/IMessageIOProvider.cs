using System.Threading.Tasks;

namespace BM.Common
{
    public interface IMessageIOProvider
    {
        void Send(Message message);

        Task Receive(Message message);
    }
}
