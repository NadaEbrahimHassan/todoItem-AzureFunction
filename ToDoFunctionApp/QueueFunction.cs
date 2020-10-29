using System;
using System.Threading.Tasks;
using FunctionAppTest.Models;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ToDoFunctionApp
{
    public static class QueueFunction
    {
        [FunctionName("QueueFunction")]
        public static async Task Run([QueueTrigger("todo", Connection = "AzureWebJobsStorage")] ToDoItem myQueueItem,
             [Blob("todo", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log)
        {
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{myQueueItem.Id}.txt");
            await blob.UploadTextAsync($"new Todo Item with id {myQueueItem.Id} is created By {myQueueItem.OwnerName}");
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
