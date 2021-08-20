using AutoMapper;
using IMapper = SFA.DAS.Provider.Events.Application.Mapping.IMapper;

namespace SFA.DAS.Provider.Events.Infrastructure.Mapping
{
    public class AutoMapperMapper : IMapper
    {
        private readonly AutoMapper.IMapper _mapper;

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
