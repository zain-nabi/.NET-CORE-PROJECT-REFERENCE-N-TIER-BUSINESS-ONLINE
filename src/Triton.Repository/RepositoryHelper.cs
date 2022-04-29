using Dapper.Contrib.Extensions;
using System.Threading.Tasks;
using System.Transactions;

namespace Triton.Repository
{
    public static class RepositoryHelper
    {
        public static async Task<bool> UpdateAsync<T>(T entity, string db) where T : class
        {
            try
            {
                // Scope transaction
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Set the connection
                await using var connection = Connection.GetOpenConnection(db);

                // Update the record async
                _ = await connection.UpdateAsync(entity).ConfigureAwait(false);

                // Save the record
                scope.Complete();

                // Return success
                return true;
            }
            catch //(Exception e)
            {
                // Log error
                return false;
            }
        }

        public static async Task<long> InsertAsync<T>(T entity, string db) where T : class
        {
            try
            {
                // Scope transaction
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Set the connection
                await using var connection = Connection.GetOpenConnection(db);

                // Insert the record async
                var id = await connection.InsertAsync(entity).ConfigureAwait(false);

                // Save the record
                scope.Complete();

                // Return success
                return id;
            }
            catch //(Exception e)
            {
                // Log error
                return 0;
            }
        }
    }
}
