namespace SFA.DAS.Provider.Events.Application.Mapping
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object source);
    }
}