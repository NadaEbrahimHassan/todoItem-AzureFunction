using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionAppTest.Models
{
    public static class ToDoMapping
    {
        public static ToDoEntity MapToDoModelToEntity(ToDoItem todo)
        {
            return new ToDoEntity()
            {
                PartitionKey = "TODO",
                RowKey = todo.Id,
                CreatedDate = todo.CreatedDate,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                OwnerName = todo.OwnerName
            };
        }
    }
}
