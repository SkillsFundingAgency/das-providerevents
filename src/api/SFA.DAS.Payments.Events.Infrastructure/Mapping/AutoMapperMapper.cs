using AutoMapper;
using IMapper = SFA.DAS.Payments.Events.Domain.Mapping.IMapper;

namespace SFA.DAS.Payments.Events.Infrastructure.Mapping
{
    public class AutoMapperMapper : IMapper
    {
        private AutoMapper.IMapper _mapper;

        public AutoMapperMapper(MapperConfiguration config)
        {
            _mapper = config.CreateMapper();
        }

        public TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }
    }
}
