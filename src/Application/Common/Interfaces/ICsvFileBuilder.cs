using ReceiptGenerator.Application.TodoLists.Queries.ExportTodos;

namespace ReceiptGenerator.Application.Common.Interfaces
{
    public interface ICsvFileBuilder
    {
        byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
    }
}