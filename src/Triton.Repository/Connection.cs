using System.Data.SqlClient;

namespace Triton.Repository
{
    public static class Connection
    {
        public static string ConnectionString { get; set; }

        public static SqlConnection GetOpenConnection(string connectionString, bool mars = false)
        {
            var cs = connectionString;
            {
                if (mars)
                {
                    var conn = new SqlConnectionStringBuilder(cs) { MultipleActiveResultSets = true };
                    cs = conn.ConnectionString;
                }
                var connection = new SqlConnection(cs);
                connection.Open();
                return connection;
            }
        }

        public static SqlConnection GetClosedConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static string GetContextInformationFromConnection(SqlConnection conn, int userId)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"DECLARE @Ctx VARBINARY(128)
                SELECT @Ctx = CONVERT(VARBINARY(128), '{userId}')
                SET CONTEXT_INFO @Ctx

                SELECT CAST(REPLACE(CAST(CONTEXT_INFO() AS VARCHAR(128)) COLLATE Latin1_General_100_BIN , 0x00, '') AS INT)";
            var result = cmd.ExecuteScalar();
            return result.ToString();
        }
    }
}