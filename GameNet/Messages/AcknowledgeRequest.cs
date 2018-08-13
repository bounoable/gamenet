using Base62;
using System;

namespace GameNet.Messages
{
    public class AcknowledgeRequest: IAcknowledgeRequest
    {
        /// <summary>
        /// The default response timeout.
        /// </summary>
        public const int DEFAULT_RESPONSE_TIMEOUT = 1000;

        /// <summary>
        /// The default retries.
        /// </summary>
        public const int DEFAULT_RETRIES = 5;

        readonly static Random _rand = new Random();
        readonly static Base62Converter _tokenGenerator = new Base62Converter();

        public string AckToken { get; }
        public int Timeout { get; } = DEFAULT_RESPONSE_TIMEOUT;
        public int Retries { get; } = DEFAULT_RETRIES;

        /// <summary>
        /// Create an acknowledge message.
        /// </summary>
        /// <param name="timeout">The response timeout.</param>
        public AcknowledgeRequest(string token = null)
        {
            if (token != null) {
                AckToken = token;
            } else {
                byte[] buffer = new byte[2];
                _rand.NextBytes(buffer);

                AckToken = _tokenGenerator.Encode(
                    BitConverter.ToString(buffer).Replace("-", string.Empty)
                );
            }
        }
    }
}