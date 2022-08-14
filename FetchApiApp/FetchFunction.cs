using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using FetchApiApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace FetchApiApp
{
    public class FetchFunction
    {
        [FunctionName("FetchFunction")]
        public static async Task Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            var client = new RestClient("https://api.publicapis.org/random?auth=null");
            var response = client.Execute(new RestRequest());

            string responseContent = response.Content;
            string statusCode = response.StatusCode.ToString();
            string isSuccess = "Is sucessful - " + response.IsSuccessful;

            string statusResult = response.IsSuccessful ? "Success" : "Failure";

            Console.WriteLine(responseContent);
            Console.WriteLine(statusCode);
            Console.WriteLine(isSuccess);

            SaveData(responseContent, statusResult).GetAwaiter().GetResult();
        }

        private static async Task<bool> SaveData(string payloadBlob, string statusResult)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            string blobId = Guid.NewGuid().ToString();
            string tableEntityId = Guid.NewGuid().ToString();
            DateTime fetchTime = DateTime.Now.ToUniversalTime();

            // BLOB SAVING

            BlobClient blobClient = new BlobClient(
                connectionString: connectionString,
                blobContainerName: Environment.GetEnvironmentVariable("BlobContainerName"),
                blobName: blobId);

            var content = Encoding.UTF8.GetBytes(payloadBlob);
            using (var ms = new MemoryStream(content))
                blobClient.Upload(ms);

            // TABLE ENTRY SAVING

            TableServiceClient tableServiceClient = new TableServiceClient(connectionString);

            var tableClient = tableServiceClient.GetTableClient(Environment.GetEnvironmentVariable("TableName"));

            LogInfo logInfo = new LogInfo()
            {
                RowKey = tableEntityId,
                PartitionKey = statusResult,
                fetchTime = fetchTime,
                blobGUID = blobId
            };

            await tableClient.AddEntityAsync(logInfo);

            return true;
        }
    }
}