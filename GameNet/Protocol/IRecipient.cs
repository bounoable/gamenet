using System.Threading.Tasks;

namespace GameNet.Protocol
{
    public interface IRecipient
    {
        /// <summary>
        /// Send data to the recipient.
        /// </summary>
        /// <param name="data">The data to send.</param>
        Task Send(byte[] data);
    }
}