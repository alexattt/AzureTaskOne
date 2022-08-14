using TaskApi.Models;

namespace TaskApi.Services
{
    public interface ITableStorageService
    {
        Task<List<LogInfo>> GetLogInfoInPeriodAsync(DateTime startDate, DateTime endDate);
        Task<LogInfo> GetLogInfoByRowKey(string partitionKey, string rowKey);
    }
}
