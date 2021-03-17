using AutoMapper;
using SFA.DAS.Provider.Events.Api.Types;
using SFA.DAS.Provider.Events.Application.Data;
using SFA.DAS.Provider.Events.Infrastructure.Mapping;

namespace SFA.DAS.Provider.Events.Api.Plumbing.Mapping
{
    public static class AutoMapperConfigurationFactory
    {
        public static MapperConfiguration CreateMappingConfig()
        {
            return new MapperConfiguration(cfg =>
            {
                DomainAutoMapperConfiguration.AddDomainMappings(cfg);

                cfg.CreateMap<CollectionPeriod, PeriodEnd>()
                    .ForMember(dst => dst.CalendarPeriod, opt => opt.Ignore())
                    .ForMember(dst => dst.ReferenceData, opt => opt.Ignore())
                    .ForMember(dst => dst.Links, opt => opt.Ignore())
                    .AfterMap((src, dst) =>
                    {
                        dst.CalendarPeriod = new CalendarPeriod
                        {
                            Month = src.CalendarMonth,
                            Year = src.CalendarYear
                        };
                        dst.ReferenceData = new ReferenceDataDetails
                        {
                            AccountDataValidAt = src.AccountDataValidAt,
                            CommitmentDataValidAt = src.CommitmentDataValidAt
                        };
                    });

                cfg.CreateMap<DataLockEvent, ObsoleteModels.DataLockEventV1>()
                    .ForMember(dst => dst.IlrPriceEffectiveDate, opt => opt.MapFrom(src => src.IlrPriceEffectiveFromDate));
                cfg.CreateMap<DataLockEventApprenticeship, ObsoleteModels.DataLockEventApprenticeshipV1>()
                   .ForMember(dst => dst.Version, opt => opt.MapFrom(
                       src => src.Version.Contains("-") ? long.Parse(src.Version.Split('-')[0]) : long.Parse(src.Version)));

                cfg.CreateMap<DataLockEventPeriod, ObsoleteModels.DataLockEventPeriodV1>()
                  .ForMember(dst => dst.ApprenticeshipVersion, opt => opt.MapFrom(
                      src => src.ApprenticeshipVersion.Contains("-") ? long.Parse(src.ApprenticeshipVersion.Split('-')[0]).ToString() : src.ApprenticeshipVersion));
            });
        }

     
    }
}