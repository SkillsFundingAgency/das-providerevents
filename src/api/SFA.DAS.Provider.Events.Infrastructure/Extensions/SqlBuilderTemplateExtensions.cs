using Dapper;

namespace SFA.DAS.Provider.Events.Infrastructure.Extensions
{
    public static class SqlBuilderExtensions
    {
        public static SqlBuilder Where(this SqlBuilder source, string sql, dynamic parameters, bool includeIf)
        {
            if (includeIf)
            {
                source.Where(sql, parameters);
            }
            return source;
        }
    }
}
