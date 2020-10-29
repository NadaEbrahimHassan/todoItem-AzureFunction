
using Microsoft.Azure.Cosmos.Table;
using System;


namespace FunctionAppTest.Models
{
    public class ToDoItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string OwnerName { get; set; }
        public bool IsCompleted { get; set;}
    }

    public class ToDoItemRequest
    {
        public string Description { get; set; }
        public string OwnerName { get; set; }
    }

    public class ToDoEntity:TableEntity
    {
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } 
        public string OwnerName { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ToDoItemUpdateModel
    {
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
