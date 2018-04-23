using AutoMapper;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Infrastructure.Mapping
{
    public static class DomainAutoMapperConfiguration
    {
        public static void AddDomainMappings(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PageOfResults<PaymentEntity>, PageOfResults<Payment>>();

            cfg.CreateMap<PaymentsDueEarningEntity, Earning>();

            cfg.CreateMap<PaymentEntity, Payment>()
                .ForMember(dst => dst.CollectionPeriod, opt => opt.Ignore())
                .ForMember(dst => dst.DeliveryPeriod, opt => opt.Ignore())
                .ForMember(dst => dst.FundingSource, opt => opt.Ignore())
                .ForMember(dst => dst.TransactionType, opt => opt.Ignore())
                .ForMember(dst => dst.EarningDetails, opt => opt.MapFrom(x => x.PaymentsDueEarningEntities))
                .AfterMap((src, dst) =>
                {
                    dst.CollectionPeriod = new NamedCalendarPeriod
                    {
                        Id = src.CollectionPeriodId.Trim(),
                        Month = src.CollectionPeriodMonth,
                        Year = src.CollectionPeriodYear
                    };
                    dst.DeliveryPeriod = new CalendarPeriod
                    {
                        Month = src.DeliveryPeriodMonth,
                        Year = src.DeliveryPeriodYear
                    };
                    dst.FundingSource = (FundingSource)src.FundingSource;
                    dst.TransactionType = (TransactionType)src.TransactionType;
                });

            cfg.CreateMap<PeriodEntity, Period>();


            cfg.CreateMap<PageOfResults<SubmissionEventEntity>, PageOfResults<SubmissionEvent>>();

            cfg.CreateMap<SubmissionEventEntity, SubmissionEvent>();


            cfg.CreateMap<PageOfResults<DataLockEventEntity>, PageOfResults<DataLockEvent>>();
            cfg.CreateMap<DataLockEventEntity, DataLockEvent>();
            cfg.CreateMap<DataLockEventErrorEntity, DataLockEventError>();
            cfg.CreateMap<DataLockEventPeriodEntity, DataLockEventPeriod>()
                .ForMember(dst => dst.Period, opt => opt.Ignore())
                .AfterMap((src, dst) =>
                {
                    dst.Period = new NamedCalendarPeriod
                    {
                        Id = src.CollectionPeriodId.Trim(),
                        Month = src.CollectionPeriodMonth,
                        Year = src.CollectionPeriodYear
                    };
                });
            cfg.CreateMap<DataLockEventApprenticeshipEntity, DataLockEventApprenticeship>();

            cfg.CreateMap<TransferEntity, AccountTransfer>()
                .ForMember(t => t.SenderAccountId, o => o.MapFrom(s => s.SendingAccountId))
                .ForMember(t => t.ReceiverAccountId, o => o.MapFrom(s => s.RecievingAccountId))
                .AfterMap((s, t) => t.Type = s.TransferType.ToString());
            cfg.CreateMap<PageOfResults<TransferEntity>, PageOfResults<AccountTransfer>>();
        }
    }
}
