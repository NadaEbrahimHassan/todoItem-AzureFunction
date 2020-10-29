using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using FunctionAppTest.Models;
using Microsoft.VisualBasic;

namespace ToDoFunctionApp
{
    public static class ToDo
    {
        static List<ToDoItem> items = new List<ToDoItem>();

        [FunctionName("CreateToDo")]
        public static async Task<IActionResult> CreateToDo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todos",Connection ="AzureWebJobsStorage")] IAsyncCollector<ToDoEntity> todoTable,
            [Queue("todo", Connection = "AzureWebJobsStorage")] IAsyncCollector<ToDoItem> todoQueue,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<ToDoItemRequest>(requestBody);

            var todoItem = new ToDoItem() { Description = data.Description, OwnerName = data.OwnerName };
            await todoTable.AddAsync(ToDoMapping.MapToDoModelToEntity(todoItem));
            await todoQueue.AddAsync(todoItem);

            return new OkObjectResult(todoItem);
        }

        [FunctionName("GetAllToDo")]
        static public async Task<ActionResult> GetAllToDo(
           [HttpTrigger(AuthorizationLevel.Anonymous,"get",Route ="todo")] HttpRequest req,
           [Table("todos",Connection ="AzureWebJobsStorage")] CloudTable todoTable,
           ILogger logger
            )
        {
            var query = new TableQuery<ToDoEntity>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            return new OkObjectResult(segment);
        }

        [FunctionName("UpdateToDoItem")]
        static public async Task<ActionResult>UpdateToDo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "todo/{id}")] HttpRequest req,
           [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
           ILogger logger,string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedItem = JsonConvert.DeserializeObject<ToDoItemUpdateModel>(requestBody);
            var operation = TableOperation.Retrieve<ToDoEntity>("TODO", id);
            var tableResult = await todoTable.ExecuteAsync(operation);
            if (tableResult.Result == null)
            {
                return new NotFoundResult();
            }

            var actualItem = (ToDoEntity)tableResult.Result;
            actualItem.IsCompleted = updatedItem.IsCompleted;
            if (!String.IsNullOrEmpty(actualItem.Description))
            {
                actualItem.Description = updatedItem.Description;
            }

            var replaceOperation = TableOperation.Replace(actualItem);
            await todoTable.ExecuteAsync(replaceOperation);
            return new OkObjectResult(actualItem);
        }

        [FunctionName("DeleteToDoItem")]
        public static async Task<ActionResult> DeleteTodoItem([HttpTrigger(AuthorizationLevel.Anonymous, "Delete", Route ="todo/{id}")] HttpRequest req,
           [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
           ILogger logger, string id)
        {
            var operation = TableOperation.Retrieve<ToDoEntity>("TODO", id);
            var tableResult = await todoTable.ExecuteAsync(operation);
            if (tableResult.Result == null)
            {
                return new NotFoundResult();
            }

            var item = (ToDoEntity)tableResult.Result;
            await todoTable.ExecuteAsync(TableOperation.Delete(item));

            return new OkObjectResult(item);
        }
    }
}
