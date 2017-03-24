using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsValidationErrorRepository : DcfsRepository, IValidationErrorRepository
    {
        private const string Source = "DataLock.ValidationError";
        private const string Columns = "Ukprn," +
                                       "LearnRefNumber," +
                                       "AimSeqNumber," +
                                       "RuleId," +
                                       "PriceEpisodeIdentifier";
        private const string SelectPriceEpisodeErrors = "SELECT " + Columns + " FROM " + Source +
            " WHERE Ukprn = @ukprn AND PriceEpisodeIdentifier = @priceEpisodeIdentifier AND LearnRefnumber = @learnRefnumber";

        public DcfsValidationErrorRepository(string connectionString)
            : base(connectionString)
        {
        }

        public ValidationErrorEntity[] GetPriceEpisodeValidationErrors(long ukprn, string priceEpisodeIdentifier, string learnRefNumber)
        {
            return Query<ValidationErrorEntity>(SelectPriceEpisodeErrors, new { ukprn, priceEpisodeIdentifier, learnRefNumber });
        }
    }
}