using System;
using System.Threading.Tasks;
using FunctionAppTest.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ToDoFunctionApp
{
    public static class scheduledFunction
    {
        [FunctionName("scheduledFunction")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable, ILogger log)
        {

            var query = new TableQuery<ToDoEntity>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            var deleted = 0;
            foreach (var item in segment)
            {
                if(item.IsCompleted)
                {
                    await todoTable.ExecuteAsync(TableOperation.Delete(item));
                    deleted++;
                }
            }
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now} and deleted {deleted} Todo Item");
        }
    }
}
