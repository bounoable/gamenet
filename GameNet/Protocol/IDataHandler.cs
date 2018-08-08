using System.Threading.Tasks;

namespace GameNet.Protocol
{
    public interface IDataHandler
    {
        /// <summary>
        /// Handle received data.
        /// </summary>
        /// <param name="data">The received data.</param>
        /// <param name="sender">The sender of the data.</param>
        void Handle(byte[] data, IRecipient sender);
    }
}