using SFA.DAS.Payments.DCFS.Infrastructure.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data;
using SFA.DAS.Provider.Events.DataLock.Domain.Data.Entities;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace SFA.DAS.Provider.Events.DataLock.Infrastructure.Data
{
    public class DcfsDataLockEventCommitmentVersionRepository : DcfsRepository, IDataLockEventCommitmentVersionRepository
    {
        private readonly string _connectionString;
        private const string Source = "Reference.DataLockEventCommitmentVersions";
        private const string Columns = "DataLockEventId," +
                                       "CommitmentVersion," +
                                       "CommitmentStartDate," +
                                       "CommitmentStandardCode," +
                                       "CommitmentProgrammeType," +
                                       "CommitmentFrameworkCode," +
                                       "CommitmentPathwayCode," +
                                       "CommitmentNegotiatedPrice," +
                                       "CommitmentEffectiveDate";
        private const string SelectEventCommitmentVersions = "SELECT " + Columns + " FROM " + Source + " WHERE DataLockEventId = @eventId";

        public DcfsDataLockEventCommitmentVersionRepository(string connectionString)
            : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public DataLockEventCommitmentVersionEntity[] GetDataLockEventCommitmentVersions(Guid eventId)
        {
            return Query<DataLockEventCommitmentVersionEntity>(SelectEventCommitmentVersions, new { eventId });
        }

        public void BulkWriteDataLockEventCommitmentVersion(DataLockEventCommitmentVersionEntity[] versions)
        {
            const int batchSize = 100;
            var skip = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                while (skip < versions.Length)
                {
                    var batch = versions.Skip(skip).Take(batchSize)
                        .Select(x => $"('{x.DataLockEventId}', '{x.CommitmentVersion}', '{x.CommitmentStartDate:yyyy-MM-dd HH:mm:ss}', " +
                                     $"{(x.CommitmentStandardCode.HasValue ? x.CommitmentStandardCode.ToString() : "NULL")}, " +
                                     $"{(x.CommitmentProgrammeType.HasValue ? x.CommitmentProgrammeType.ToString() : "NULL")}, " +
                                     $"{(x.CommitmentFrameworkCode.HasValue ? x.CommitmentFrameworkCode.ToString() : "NULL")}, " +
                                     $"{(x.CommitmentPathwayCode.HasValue ? x.CommitmentPathwayCode.ToString() : "NULL")}, " +
                                     $"{x.CommitmentNegotiatedPrice}, '{x.CommitmentEffectiveDate:yyyy-MM-dd HH:mm:ss}')")
                        .Aggregate((x, y) => $"{x}, {y}");
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO DataLockEvents.DataLockEventCommitmentVersions " +
                                              "(DataLockEventId, CommitmentVersion, CommitmentStartDate, CommitmentStandardCode, " +
                                              "CommitmentProgrammeType, CommitmentFrameworkCode, CommitmentPathwayCode, " +
                                              "CommitmentNegotiatedPrice, CommitmentEffectiveDate) " +
                                              $"VALUES {batch}";
                        command.ExecuteNonQuery();
                    }

                    skip += batchSize;
                }
            }
        }
    }
}