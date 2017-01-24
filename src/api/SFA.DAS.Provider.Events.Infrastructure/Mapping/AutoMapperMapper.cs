using AutoMapper;

namespace SFA.DAS.Provider.Events.Infrastructure.Mapping
{
    public class AutoMapperMapper : Provider.Events.Domain.Mapping.IMapper
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
