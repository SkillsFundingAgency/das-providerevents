using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Provider.Events.DataLockEventWorker.IntegrationTests
{
    class TestDataHelper
    {
        static readonly string _connectionString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

        public void Clean()
        {
        }

        public IList<DataLockEvent> GetAllEvents()
        {
            return null;
        }

        public void AddLearningProvider(long ukprn)
        {
        }

        public void AddCommitment(long id,
            long ukprn,
            string learnerRefNumber,
            int aimSequenceNumber = 1,
            long uln = 0L,
            DateTime startDate = default(DateTime),
            DateTime endDate = default(DateTime),
            decimal agreedCost = 15000m,
            long? standardCode = null,
            int? programmeType = null,
            int? frameworkCode = null,
            int? pathwayCode = null,
            bool passedDataLock = true)
        {
            var minStartDate = new DateTime(2017, 4, 1);

            if (uln == 0)
            {
                uln = 123456;
            }

            if (!standardCode.HasValue && !frameworkCode.HasValue)
            {
                standardCode = 27;
            }

            if (startDate < minStartDate)
            {
                startDate = minStartDate;
            }

            if (endDate < startDate)
            {
                endDate = startDate.AddYears(1);
            }

            Execute("INSERT INTO Reference.DasCommitments " +
                    "(CommitmentId,VersionId,AccountId,Uln,Ukprn,StartDate,EndDate,AgreedCost,StandardCode,ProgrammeType,FrameworkCode,PathwayCode,PaymentStatus,PaymentStatusDescription,Priority,EffectiveFrom) " +
                    "VALUES " +
                    "(@id, 1, '123', @uln, @ukprn, @startDate, @endDate, @agreedCost, @standardCode, @programmeType, @frameworkCode, @pathwayCode, 1, 'Active', 1, @startDate)",
                    new { id, uln, ukprn, startDate, endDate, agreedCost, standardCode, programmeType, frameworkCode, pathwayCode });

            var priceEpisodeIdentifier = $"99-99-99-{startDate.ToString("yyyy-MM-dd")}";

            Execute("INSERT INTO DataLock.PriceEpisodeMatch "
                    + "(Ukprn,LearnRefNumber,AimSeqNumber,CommitmentId,PriceEpisodeIdentifier,IsSuccess) "
                    + "VALUES "
                    + "(@ukprn,@learnerRefNumber,@aimSequenceNumber,@id,@priceEpisodeIdentifier,@isSuccess)",
                    new { id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, isSuccess = passedDataLock });

            //var censusDate = LastDayOfMonth(startDate);
            //var period = 1;

            //while (censusDate <= endDate && period <= 12)
            //{
            //    foreach (var traxType in Enum.GetValues(typeof(TransactionTypesFlag)))
            //    {
            //        AddPriceEpisodePeriodMatch(id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, (int)traxType, passedDataLock);
            //    }

            //    censusDate = LastDayOfMonth(censusDate.AddMonths(1));
            //    period++;
            //}

            //if (endDate != LastDayOfMonth(endDate) && period <= 12)
            //{
            //    foreach (var traxType in Enum.GetValues(typeof(TransactionTypesFlag)))
            //    {
            //        AddPriceEpisodePeriodMatch(id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier, period, (int)traxType, passedDataLock);
            //    }
            //}

            if (!passedDataLock)
            {
                Execute("INSERT INTO DataLock.ValidationError "
                      + "(Ukprn, LearnRefNumber, AimSeqNumber, RuleId, PriceEpisodeIdentifier) "
                      + "VALUES "
                      + "(@ukprn, @learnerRefNumber, @aimSequenceNumber, 'DLOCK_07', @priceEpisodeIdentifier)",
                      new { id, ukprn, learnerRefNumber, aimSequenceNumber, priceEpisodeIdentifier });
            }
        }

            //private static int DaysInMonth(DateTime value)
            //{
            //    return DateTime.DaysInMonth(value.Year, value.Month);
            //}

            //private static DateTime LastDayOfMonth(DateTime value)
            //{
            //    return new DateTime(value.Year, value.Month, DaysInMonth(value));
            //}

        internal void AddIlrDataForCommitment(long? commitmentId,
                                                     string learnerRefNumber,
                                                     int aimSequenceNumber = 1)
        {
            Execute("INSERT INTO Rulebase.AEC_ApprenticeshipPriceEpisode "
                    + "(LearnRefNumber, PriceEpisodeIdentifier, EpisodeEffectiveTNPStartDate, EpisodeStartDate, "
                    + "PriceEpisodeAimSeqNumber, PriceEpisodePlannedEndDate, PriceEpisodeTotalTNPPrice, TNP1, TNP2) "
                    + "SELECT "
                    + "@learnerRefNumber, "
                    + "'99-99-99-' + CONVERT(char(10), StartDate, 126), "
                    + "StartDate, "
                    + "StartDate, "
                    + "@aimSequenceNumber, "
                    + "EndDate, "
                    + "AgreedCost, "
                    + "AgreedCost * 0.8, "
                    + "AgreedCost * 0.2 "
                    + "FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber, aimSequenceNumber });

            Execute("INSERT INTO Valid.Learner "
                    + "(LearnRefNumber, ULN, Ethnicity, Sex, LLDDHealthProb) "
                    + "SELECT @learnerRefNumber,Uln,0,0,0 FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber });

            Execute("INSERT INTO Valid.LearningDelivery "
                    + "(LearnRefNumber, LearnAimRef, AimType, AimSeqNumber, LearnStartDate, LearnPlanEndDate, FundModel, StdCode, ProgType, FworkCode, PwayCode) "
                    + "SELECT @learnerRefNumber, 'ZPROG001', 1, @aimSequenceNumber, StartDate, EndDate, 36, StandardCode, ProgrammeType, FrameworkCode, PathwayCode FROM Reference.DasCommitments "
                    + "WHERE CommitmentId = @commitmentId",
                new { commitmentId, learnerRefNumber, aimSequenceNumber });
        }

        public void AddDataLockValidationError(long ukprn, string learnRefNumber, long? aimSeqNumber, string priceEpisodeIdentifier, string ruleId)
        {

        }

        public void AddDataLockPricePeriodMatch(long ukprn, string learnRefNumber, long? aimSeqNumber, string priceEpisodeIdentifier, long commitmentId, bool isSuccess)
        {

        }

        private static void Execute(string command, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
                connection.Execute(command, param);
        }
    }
}
