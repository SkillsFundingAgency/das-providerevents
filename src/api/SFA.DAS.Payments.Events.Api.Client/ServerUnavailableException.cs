using System;
using System.Net.Sockets;

namespace SFA.DAS.Payments.Events.Api.Client
{
    public class ServerUnavailableException : ApiException
    {
        public ServerUnavailableException(Exception innerException, SocketError reason)
            : base("Payments Events API is currently unavailable (" + reason + ")", innerException)
        {
            Reason = reason;
        }
        public SocketError Reason { get; }
    }
}