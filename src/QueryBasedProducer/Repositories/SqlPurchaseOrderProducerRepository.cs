using Dapper;
using QueryBasedProducer.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace QueryBasedProducer.Repositories
{
    public class SqlPurchaseOrderProducerRepository : IPurchaseOrderProducerRepository
    {
        public IDbConnection GetConnection()
        {
            // should always come from injected configuration values but because this is a demo...
            //var conn = new SqlConnection("Server=localhost;Database=mrjb_querybased;User Id=sa;Password=NyLct4D@7K{s;");
            var conn = new SqlConnection("Server=localhost;Database=mrjb_batchoperations;User Id=sa;Password=NyLct4D@7K{s;");
            return conn;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderForProcessingAsync()
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    conn.Open();
                    var data = (await conn.QueryAsync<PurchaseOrder>("uspBatchOperationGetData", commandType: CommandType.StoredProcedure)).SingleOrDefault();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task TombstonePurchaseOrder(int id)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    conn.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@PurchaseOrderId", id);
                    await conn.ExecuteAsync("uspBatchOperationTombstone", parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
