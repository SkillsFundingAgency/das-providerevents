using MediatR;
using SFA.DAS.Provider.Events.DataLock.Domain;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;

namespace SFA.DAS.Provider.Events.DataLock.Application.GetCurrentCollectionPeriod
{
    public class GetCurrentCollectionPeriodHandler: IRequestHandler<GetCurrentCollectionPeriodRequest,GetCurrentCollectionPeriodResposne>
    {
        private readonly ICollectionPeriodRepository _collectionPeriodRepository;

        public GetCurrentCollectionPeriodHandler(ICollectionPeriodRepository collectionPeriodRepository)
        {
            _collectionPeriodRepository = collectionPeriodRepository;
        }

        public GetCurrentCollectionPeriodResposne Handle(GetCurrentCollectionPeriodRequest message)
        {
            var collectionPeriod = _collectionPeriodRepository.GetCurrentCollectionPeriod();

            if (collectionPeriod == null)
            {
                return new GetCurrentCollectionPeriodResposne();
            }

            var getCurrentCollectionPeriodResposne = new GetCurrentCollectionPeriodResposne
            {
                CollectionPeriod = new CollectionPeriod
                {
                    Month = collectionPeriod.Month,
                    Year = collectionPeriod.Year,
                    Name = collectionPeriod.Name
                }
            };

            return getCurrentCollectionPeriodResposne;
        }
    }
}
