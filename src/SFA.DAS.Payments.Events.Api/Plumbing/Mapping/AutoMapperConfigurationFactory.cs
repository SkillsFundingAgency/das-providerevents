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
            });
        }
    }
}