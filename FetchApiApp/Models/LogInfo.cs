using System;
using Azure;
using Azure.Data.Tables;

namespace FetchApiApp.Models
{
    public class LogInfo : ITableEntity
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        public DateTime fetchTime { get; set; }
        public string blobGUID { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
