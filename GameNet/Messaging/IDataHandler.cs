using System.Threading.Tasks;

namespace GameNet.Messaging
{
    public interface IDataHandler
    {
        /// <summary>
        /// Handle received data.
        /// </summary>
        /// <param name="data">The received data.</param>
        void Handle(byte[] data);
    }
}