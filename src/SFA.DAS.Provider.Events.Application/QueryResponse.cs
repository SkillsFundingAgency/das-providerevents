﻿namespace SFA.DAS.Provider.Events.Application
{
    public abstract class QueryResponse<T> : Response
    {
        public T Result { get; set; }
    }
}
