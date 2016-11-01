namespace SFA.DAS.Payments.Events.Domain.Mapping
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object source);
    }
}