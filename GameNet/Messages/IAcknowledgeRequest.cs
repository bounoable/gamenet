using System;

namespace GameNet.Messages
{
    public interface IAcknowledgeRequest
    {
        /// <summary>
        /// The unique acknowledge token of the message.
        /// </summary>
        string AckToken { get; }

        /// <summary>
        /// The response timeout.
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// The maximum number of retries.
        /// </summary>
        int Retries { get; }
    }
}