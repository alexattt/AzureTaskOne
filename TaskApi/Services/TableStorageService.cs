using Azure;
using Azure.Data.Tables;
using TaskApi.Models;

namespace TaskApi.Services
{
    public class TableStorageService : ITableStorageService
    {
        private const string TableName = "apilogdata";
        private readonly IConfiguration _configuration;

        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["AzureStorageConn"]);
            var tableClient = serviceClient.GetTableClient(TableName);

            return tableClient;
        }

        public async Task<List<LogInfo>> GetLogInfoInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var tableClient = await GetTableClient();

            Pageable<LogInfo> queryResultsLINQ = tableClient.Query<LogInfo>(ent => ent.fetchTime >= startDate && ent.fetchTime <= endDate);

            return queryResultsLINQ.ToList();
        }

        public async Task<LogInfo> GetLogInfoByRowKey(string partitionKey, string rowKey)
        {
            var tableClient = await GetTableClient();

            LogInfo logInfo = tableClient.GetEntity<LogInfo>(partitionKey, rowKey);

            return logInfo;
        }
    }
}
