using Microsoft.AspNetCore.Mvc;
using TaskApi.Models;
using TaskApi.Services;

namespace TaskApi.Controllers
{
    // decided to create one Controller, since in this case there are only two services and just a few http GET requests
    // otherwise there would be Controller for each Service and Controller woould probably contain various requests

    [ApiController]
    [Route("api/[controller]")]
    public class AzureStorageController : ControllerBase
    {
        private readonly ITableStorageService _storageService;
        private readonly IBlobStorageInterface _blobStorageService;

        public AzureStorageController(ITableStorageService storageService, IBlobStorageInterface blobStorageService)
        {
            _storageService = storageService;
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<List<LogInfo>> GetLogsInPeriodAsync([FromQuery] DateTime startDate, DateTime endDate)
        {
            return await _storageService.GetLogInfoInPeriodAsync(startDate, endDate);
        }

        [HttpGet("logByPartitionAndRowKey")]
        public async Task<LogInfo> GetLogAsync([FromQuery] string partitionKey, string rowKey)
        {
            LogInfo logInfo = await _storageService.GetLogInfoByRowKey(partitionKey, rowKey);

            return logInfo;
        }

        [HttpGet("blobPayloadForLog")]
        public async Task<string> GetBlobContentForLogEntry([FromQuery] string partitionKey, string rowKey)
        {
            LogInfo logInfo = await GetLogAsync(partitionKey, rowKey);

            string blobContent = await _blobStorageService.GetBlobContent(logInfo.blobGUID);

            return blobContent;
        }
    }
}
