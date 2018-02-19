using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.SqlServer.Server;
using SFA.DAS.Provider.Events.Application.Data.Entities;

namespace SFA.DAS.Provider.Events.Infrastructure.Data
{
    internal class DataLockEntityTableValueParameter : SqlMapper.IDynamicParameters
    {
        private readonly IEnumerable<DataLockEntity> _dataLocks;

        private static readonly SqlMetaData _ukprnMetaData = new SqlMetaData("Ukprn", SqlDbType.BigInt);
        private static readonly SqlMetaData _learnRefNumberMetaData = new SqlMetaData("LearnerReferenceNumber", SqlDbType.VarChar, 12);
        private static readonly SqlMetaData _priceEpisodeIdMetaData = new SqlMetaData("PriceEpisodeIdentifier", SqlDbType.VarChar, 25);
        private static readonly SqlMetaData _aimSequenceNumberMetaData = new SqlMetaData("AimSequenceNumber", SqlDbType.BigInt);
        private static readonly SqlMetaData _errorCodesMetaData = new SqlMetaData("ErrorCodes", SqlDbType.NVarChar, SqlMetaData.Max);
        private static readonly SqlMetaData _commitmentsMetaData = new SqlMetaData("CommitmentVersions", SqlDbType.NVarChar, SqlMetaData.Max);
        private static readonly SqlMetaData _deletedMetaData = new SqlMetaData("DeletedUtc", SqlDbType.DateTime);

        public DataLockEntityTableValueParameter(IEnumerable<DataLockEntity> dataLocks)
        {
            _dataLocks = dataLocks;
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            var sqlCommand = (SqlCommand) command;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            var items = new List<SqlDataRecord>();
            foreach (var param in _dataLocks)
            {
                var rec = new SqlDataRecord(_ukprnMetaData, _learnRefNumberMetaData, _priceEpisodeIdMetaData, _aimSequenceNumberMetaData, _errorCodesMetaData, _commitmentsMetaData, _deletedMetaData);
                rec.SetInt64(0, param.Ukprn);
                if (param.LearnerReferenceNumber != null)
                    rec.SetString(1, param.LearnerReferenceNumber);
                rec.SetString(2, param.PriceEpisodeIdentifier);
                if (param.AimSequenceNumber.HasValue)
                    rec.SetInt64(3, param.AimSequenceNumber.Value);
                if (param.ErrorCodes != null)
                    rec.SetString(4, param.ErrorCodes);
                if (param.DeletedUtc.HasValue)
                    rec.SetDateTime(6, param.DeletedUtc.Value);
                items.Add(rec);
            }

            var p = sqlCommand.Parameters.Add("@dataLocks", SqlDbType.Structured);
            p.Direction = ParameterDirection.Input;
            p.TypeName = "[DataLockEvents].[DataLockEntity]";
            p.Value = items;
        }
    }
}
