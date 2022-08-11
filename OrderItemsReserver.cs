using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderItemsReserver
{
    public static class OrderItemsReserver
    {
        [FunctionName("OrderItemsReserver")]
        public static async Task Run(
            [ServiceBusTrigger("orderitemsreserver", Connection = "ServiceBusConnection")]
            string queueItem,
            ILogger log)
        {
            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            Stream myBlob = new MemoryStream(Encoding.UTF8.GetBytes(queueItem));
            var fileName = $"order_{DateTime.Now.ToFileTime()}.json";
            try
            {
                var blobClient = new BlobContainerClient(Connection, containerName);
                var blob = blobClient.GetBlobClient(fileName);
                await blob.UploadAsync(myBlob);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var httpClient = new HttpClient();
                string logicAppUri = Environment.GetEnvironmentVariable("LogicAppUri");
                var errorMessage = new ErrorMessage { Message = e.Message };
                await httpClient.PostAsync(logicAppUri, new StringContent(JsonConvert.SerializeObject(errorMessage), Encoding.UTF8, "application/json"));
            }
        }
    }

    public class ErrorMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
