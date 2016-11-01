using AutoMapper;
using SFA.DAS.Payments.Events.Infrastructure.Mapping;

namespace SFA.DAS.Payments.Events.Api.Plumbing.Mapping
{
    public static class AutoMapperConfigurationFactory
    {
        public static MapperConfiguration CreateMappingConfig()
        {
            return new MapperConfiguration(cfg =>
            {
                DomainAutoMapperConfiguration.AddDomainMappings(cfg);

                cfg.CreateMap<Domain.CalendarPeriod, Types.CalendarPeriod>();
                cfg.CreateMap<Domain.NamedCalendarPeriod, Types.NamedCalendarPeriod>();
                cfg.CreateMap<Domain.Payment, Types.Payment>()
                    .ForMember(dst => dst.FundingSource, opt => opt.Ignore())
                    .ForMember(dst => dst.TransactionType, opt => opt.Ignore())
                    .AfterMap((src, dst) =>
                    {
                        dst.FundingSource = (Types.FundingSource)(int)src.FundingSource;
                        dst.TransactionType = (Types.TransactionType)(int)src.TransactionType;
                    });
                cfg.CreateMap<Domain.PageOfResults<Domain.Payment>, Types.PageOfResults<Types.Payment>>();

                cfg.CreateMap<Domain.Period, Types.PeriodEnd>()
                    .ForMember(dst => dst.CalendarPeriod, opt => opt.Ignore())
                    .ForMember(dst => dst.ReferenceData, opt => opt.Ignore())
                    .ForMember(dst => dst.Links, opt => opt.Ignore())
                    .AfterMap((src, dst) =>
                    {
                        dst.CalendarPeriod = new Types.CalendarPeriod
                        {
                            Month = src.CalendarMonth,
                            Year = src.CalendarYear
                        };
                        dst.ReferenceData = new Types.ReferenceDataDetails
                        {
                            AccountDataValidAt = src.AccountDataValidAt,
                            CommitmentDataValidAt = src.CommitmentDataValidAt
                        };
                    });
            });
        }
    }
}