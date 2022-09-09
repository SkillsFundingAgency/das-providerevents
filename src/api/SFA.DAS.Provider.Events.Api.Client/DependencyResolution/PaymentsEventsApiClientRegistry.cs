using StructureMap;

namespace SFA.DAS.Provider.Events.Api.Client.DependencyResolution
{
    public class PaymentsEventsApiClientRegistry : Registry
    {
        public PaymentsEventsApiClientRegistry()
        {
            For<IPaymentsEventsApiClient>().Use(c => c.GetInstance<IPaymentsEventsApiClientFactory>().CreateClient(null)).Singleton();
            For<IPaymentsEventsApiClientFactory>().Use<PaymentsEventsApiClientFactory>();
        }
    }
}
