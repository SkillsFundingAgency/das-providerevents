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

                cfg.CreateMap<Domain.Payment, Types.Payment>();
                cfg.CreateMap<Domain.PageOfResults<Domain.Payment>, Types.PageOfResults<Types.Payment>>();
            });
        }
    }
}