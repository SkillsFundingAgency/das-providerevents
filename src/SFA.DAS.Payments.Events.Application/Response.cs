using System;

namespace SFA.DAS.Payments.Events.Application
{
    public abstract class Response
    {
        public bool IsValid { get; set; }
        public Exception Exception { get; set; }
    }
}
