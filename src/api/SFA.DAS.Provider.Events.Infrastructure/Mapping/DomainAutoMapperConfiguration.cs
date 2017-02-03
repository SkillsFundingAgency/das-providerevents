using AutoMapper;
using SFA.DAS.Provider.Events.Domain;
using SFA.DAS.Provider.Events.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.Infrastructure.Mapping
{
    public static class DomainAutoMapperConfiguration
    {
        public static void AddDomainMappings(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PageOfEntities<PaymentEntity>, PageOfResults<Payment>>();

            cfg.CreateMap<PaymentEntity, Payment>()
                .ForMember(dst => dst.CollectionPeriod, opt => opt.Ignore())
                .ForMember(dst => dst.DeliveryPeriod, opt => opt.Ignore())
                .ForMember(dst => dst.FundingSource, opt => opt.Ignore())
                .ForMember(dst => dst.TransactionType, opt => opt.Ignore())
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


            cfg.CreateMap<PageOfEntities<SubmissionEventEntity>, PageOfResults<SubmissionEvent>>();

            cfg.CreateMap<SubmissionEventEntity, SubmissionEvent>();
        }
    }
}
