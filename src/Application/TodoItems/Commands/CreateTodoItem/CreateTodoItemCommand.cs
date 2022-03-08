﻿using MediatR;
using ReceiptGenerator.Application.Common.Interfaces;
using ReceiptGenerator.Domain.Entities;
using ReceiptGenerator.Domain.Events;

namespace ReceiptGenerator.Application.TodoItems.Commands.CreateTodoItem
{
    public class CreateTodoItemCommand : IRequest<int>
    {
        public int ListId { get; set; }

        public string? Title { get; set; }
    }

    public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateTodoItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoItem
            {
                ListId = request.ListId,
                Title = request.Title,
                Done = false
            };

            entity.DomainEvents.Add(new TodoItemCreatedEvent(entity));

            _context.TodoItems.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}