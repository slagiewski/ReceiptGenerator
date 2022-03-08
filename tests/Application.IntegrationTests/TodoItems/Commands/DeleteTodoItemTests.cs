using FluentAssertions;
using NUnit.Framework;
using ReceiptGenerator.Application.Common.Exceptions;
using ReceiptGenerator.Application.TodoItems.Commands.CreateTodoItem;
using ReceiptGenerator.Application.TodoItems.Commands.DeleteTodoItem;
using ReceiptGenerator.Application.TodoLists.Commands.CreateTodoList;
using ReceiptGenerator.Domain.Entities;
using static Testing;

namespace ReceiptGenerator.Application.IntegrationTests.TodoItems.Commands
{
    public class DeleteTodoItemTests : TestBase
    {
        [Test]
        public async Task ShouldRequireValidTodoItemId()
        {
            var command = new DeleteTodoItemCommand { Id = 99 };

            await FluentActions.Invoking(() =>
                SendAsync(command)).Should().ThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task ShouldDeleteTodoItem()
        {
            var listId = await SendAsync(new CreateTodoListCommand
            {
                Title = "New List"
            });

            var itemId = await SendAsync(new CreateTodoItemCommand
            {
                ListId = listId,
                Title = "New Item"
            });

            await SendAsync(new DeleteTodoItemCommand
            {
                Id = itemId
            });

            var item = await FindAsync<TodoItem>(itemId);

            item.Should().BeNull();
        }
    }
}