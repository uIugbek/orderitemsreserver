using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OrderItemReserver.Services;

namespace OrderItemReserver
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
                // var mailService = new MailService();
                // var file = new FormFile(myBlob, 0, myBlob.Length, "streamFile", fileName);
                // await mailService.SendEmailAsync(new MailRequest
                // {
                //     ToEmail = "u.tangmatov@gmail.com",
                //     Subject = "eShopOnWeb",
                //     Body = "The order details cannot be saved to Azure Blob Storage",
                //     Attachments = new List<IFormFile> { file }
                // });
                Console.WriteLine(e);
            }
        }

        // [FunctionName("OrderItemsReserver")]
        // public static async Task<IActionResult> Run(
        //     [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
        //     HttpRequest req,
        //     ILogger log)
        // {
        //     log.LogInformation("C# HTTP trigger function processed a request.");
        //
        //     string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //     string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        //     string containerName = Environment.GetEnvironmentVariable("ContainerName");
        //     Stream myBlob = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
        //     var blobClient = new BlobContainerClient(Connection, containerName);
        //     var blob = blobClient.GetBlobClient($"order_{DateTime.Now.ToFileTime()}.json");
        //     await blob.UploadAsync(myBlob);
        //     return new OkObjectResult("file uploaded successfully");
        // }
    }
}
