using System.Threading.Tasks;

namespace BM.Common
{
    public interface IMessageIOProvider
    {
        void Send(string message);

        void Receive(string message);
    }
}
