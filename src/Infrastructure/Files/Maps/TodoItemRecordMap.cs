using CsvHelper.Configuration;
using ReceiptGenerator.Application.TodoLists.Queries.ExportTodos;
using System.Globalization;

namespace ReceiptGenerator.Infrastructure.Files.Maps
{
    public class TodoItemRecordMap : ClassMap<TodoItemRecord>
    {
        public TodoItemRecordMap()
        {
            AutoMap(CultureInfo.InvariantCulture);

            Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
        }
    }
}