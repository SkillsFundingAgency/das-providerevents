﻿using System;

namespace SFA.DAS.Provider.Events.Api.Client
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}