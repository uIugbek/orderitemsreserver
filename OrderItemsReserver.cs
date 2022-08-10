using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

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
            }
        }
    }
}
