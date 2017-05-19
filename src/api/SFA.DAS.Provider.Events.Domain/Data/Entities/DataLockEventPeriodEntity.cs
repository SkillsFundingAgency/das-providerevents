﻿using System;

namespace SFA.DAS.Provider.Events.Domain.Data.Entities
{
    public class DataLockEventPeriodEntity
    {
        public Guid DataLockEventId { get; set; }
        public long ApprenticeshipVersion { get; set; }
        public string CollectionPeriodId { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public bool IsPayable { get; set; }
        public int TransactionType { get; set; }
    }
}