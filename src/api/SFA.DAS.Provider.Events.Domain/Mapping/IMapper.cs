namespace SFA.DAS.Provider.Events.Domain.Mapping
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object source);
    }
}