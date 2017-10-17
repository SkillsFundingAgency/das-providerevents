using Dapper;

namespace SFA.DAS.Provider.Events.Infrastructure.Extensions
{
    public static class SqlBuilderExtensions
    {
        public static SqlBuilder Where(this SqlBuilder source, bool onlyIncludeIfTrue, string sql, dynamic parameters = null)
        {
            if (onlyIncludeIfTrue)
            {
                source.Where(sql, parameters);
            }
            return source;
        }
    }
}
