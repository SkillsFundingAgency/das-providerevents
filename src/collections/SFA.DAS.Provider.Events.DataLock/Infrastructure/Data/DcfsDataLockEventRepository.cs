using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SFA.DAS.Provider.Events.DataLock.Domain;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventRepository : DcfsRepository, IDataLockEventRepository
    {
        private readonly string _connectionString;
        private const string Source = "Reference.DataLockEvents";

        private const string Columns = "Id," +
                                       "DataLockEventId," +
                                       "ProcessDateTime," +
                                       "IlrFileName," +
                                       "SubmittedDateTime," +
                                       "AcademicYear," +
                                       "UKPRN," +
                                       "ULN," +
                                       "LearnRefNumber," +
                                       "AimSeqNumber," +
                                       "PriceEpisodeIdentifier," +
                                       "CommitmentId," +
                                       "EmployerAccountId," +
                                       "EventSource," +
                                       "HasErrors," +
                                       "IlrStartDate," +
                                       "IlrStandardCode," +
                                       "IlrProgrammeType," +
                                       "IlrFrameworkCode," +
                                       "IlrPathwayCode," +
                                       "IlrTrainingPrice," +
                                       "IlrEndpointAssessorPrice," +
                                       "IlrPriceEffectiveFromDate," +
                                       "IlrPriceEffectiveToDate";
        private const string SelectLastSeenEvents = "SELECT " + Columns + " FROM " + Source;
        private const string SelectProviderLastSeenEvents = SelectLastSeenEvents + " WHERE Ukprn = @ukprn";


        public DcfsDataLockEventRepository(string connectionString)
            : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public DataLockEventEntity[] GetProviderLastSeenEvents(long ukprn)
        {
            return Query<DataLockEventEntity>(SelectProviderLastSeenEvents, new { ukprn });
        }

        public void BulkWriteDataLockEvents(DataLockEventEntity[] events)
        {
            const int batchSize = 100;
            var skip = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                while (skip < events.Length)
                {
                    var batch = events.Skip(skip).Take(batchSize)
                        .Select(x => $"('{x.DataLockEventId}','{x.ProcessDateTime:yyyy-MM-dd HH:mm:ss}', '{x.IlrFileName}', '{x.SubmittedDateTime:yyyy-MM-dd HH:mm:ss}', " +
                                     $"'{x.AcademicYear}', '{x.Ukprn}', '{x.Uln}', '{x.LearnRefnumber}', '{x.AimSeqNumber}', '{x.PriceEpisodeIdentifier}', " +
                                     $"'{x.CommitmentId}', '{x.EmployerAccountId}', '{(int)x.EventSource}', '{x.HasErrors}', {ConvertNullableToInsertValue(x.IlrStartDate)}, " +
                                     $"{ConvertNullableToInsertValue(x.IlrStandardCode)}, {ConvertNullableToInsertValue(x.IlrProgrammeType)}, " +
                                     $"{ConvertNullableToInsertValue(x.IlrFrameworkCode)}, {ConvertNullableToInsertValue(x.IlrPathwayCode)}, " +
                                     $"{ConvertNullableToInsertValue(x.IlrTrainingPrice)}, {x.IlrEndpointAssessorPrice}, " +
                                     $"{ConvertNullableToInsertValue(x.IlrPriceEffectiveFromDate)}, {ConvertNullableToInsertValue(x.IlrPriceEffectiveToDate)})")
                        .Aggregate((x, y) => $"{x}, {y}");
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"INSERT INTO DataLockEvents.DataLockEvents ({Columns.Substring(3)}) VALUES {batch}";
                        command.ExecuteNonQuery();
                    }

                    skip += batchSize;
                }
            }
        }


        private string ConvertNullableToInsertValue(DateTime? value)
        {
            if (!value.HasValue)
            {
                return "NULL";
            }

            return $"'{value.Value:yyyy-MM-dd HH:mm:ss}'";
        }
        private string ConvertNullableToInsertValue(int? value)
        {
            if (!value.HasValue)
            {
                return "NULL";
            }

            return $"'{value.Value}'";
        }
        private string ConvertNullableToInsertValue(long? value)
        {
            if (!value.HasValue)
            {
                return "NULL";
            }

            return $"'{value.Value}'";
        }
        private string ConvertNullableToInsertValue(decimal? value)
        {
            if (!value.HasValue)
            {
                return "NULL";
            }

            return $"'{value.Value}'";
        }
    }
}