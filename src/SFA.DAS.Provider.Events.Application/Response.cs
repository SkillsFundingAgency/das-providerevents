using System;

namespace SFA.DAS.Provider.Events.Application
{
    public abstract class Response
    {
        public bool IsValid { get; set; }
        public Exception Exception { get; set; }
    }
}
