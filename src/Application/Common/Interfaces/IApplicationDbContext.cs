using Microsoft.EntityFrameworkCore;
using ReceiptGenerator.Domain.Entities;

namespace ReceiptGenerator.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<TodoList> TodoLists { get; }

        DbSet<TodoItem> TodoItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}