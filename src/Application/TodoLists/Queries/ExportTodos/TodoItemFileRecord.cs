using ReceiptGenerator.Application.Common.Mappings;
using ReceiptGenerator.Domain.Entities;

namespace ReceiptGenerator.Application.TodoLists.Queries.ExportTodos
{
    public class TodoItemRecord : IMapFrom<TodoItem>
    {
        public string? Title { get; set; }

        public bool Done { get; set; }
    }
}