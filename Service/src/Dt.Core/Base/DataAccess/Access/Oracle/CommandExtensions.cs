using System.Data;

namespace Dt.Core
{
    internal static class CommandExtensions
    {
        public static bool IsWrapped(this IDbConnection connection)
        {
            return string.Compare(connection.GetType().Name, "OracleConnection", StringComparison.InvariantCulture) != 0;
        }

        public static bool IsWrapped(this IDbCommand connection)
        {
            return string.Compare(connection.GetType().Name, "OracleCommand", StringComparison.InvariantCulture) != 0;
        }
    }
}
