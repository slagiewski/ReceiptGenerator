using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReceiptGenerator.Application.Common.Interfaces;
using ReceiptGenerator.Domain.Common;
using ReceiptGenerator.Domain.Entities;
using ReceiptGenerator.Infrastructure.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ReceiptGenerator.Infrastructure.Persistence
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions,
            ICurrentUserService currentUserService,
            IDomainEventService domainEventService,
            IDateTime dateTime) : base(options, operationalStoreOptions)
        {
            _currentUserService = currentUserService;
            _domainEventService = domainEventService;
            _dateTime = dateTime;
        }

        public DbSet<TodoList> TodoLists => Set<TodoList>();

        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<ReceiptSeller> ReceiptSellers { get; set; }



        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.Created = _dateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModified = _dateTime.Now;
                        break;
                }
            }

            var events = ChangeTracker.Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .ToArray();

            var result = await base.SaveChangesAsync(cancellationToken);

            await DispatchEvents(events);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        private async Task DispatchEvents(DomainEvent[] events)
        {
            foreach (var @event in events)
            {
                @event.IsPublished = true;
                await _domainEventService.Publish(@event);
            }
        }

        public class Receipt
        {
            public int Id { get; set; }

            //generate unique index per sellerId
            [Required]
            public string ReceiptNo { get; set; }

            [Required]
            [MinLength(1)]
            public List<ReceiptItem> Items { get; set; }


            public ReceiptSeller Seller { get; set; }
            [Required]
            public int SellerId { get; set; }

            public Buyer Buyer { get; set; }
            [Required]
            public int BuyerId { get; set; }

            [Required]
            public DateTime ReceiptDate { get; set; }
            [Required]
            public string ReceiptCity { get; set; }

            public string? AdditionalDescription { get; set; }

            [Required]
            [Precision(18, 2)]
            public decimal TotalAmount { get; set; }

            [Required]
            public DateTime CreatedDate { get; set; }
            [Required]
            public DateTime ModifiedDate { get; set; }
        }

        public class Buyer
        {
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string? Regon { get; set; }
            public string? Nip { get; set; }

            [Required]
            public string City { get; set; }
            public string? AddressDetails { get; set; }
            [Required]
            public string ZipCode { get; set; }
            [Required]
            public string Street { get; set; }

            [Required]
            public DateTime CreatedDate { get; set; }
            [Required]
            public DateTime ModifiedDate { get; set; }
        }

        public class ReceiptSeller
        {
            public int Id { get; set; }

            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string Street { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string ZipCode { get; set; }

            public string BankAccountNo { get; set; }
            [Required]
            public DateTime CreatedDate { get; set; }
            [Required]
            public DateTime ModifiedDate { get; set; }
        }

        public class ReceiptItem
        {
            public int Id { get; set; }

            [Required]
            public int ReceiptId { get; set; }
            public Receipt Receipt { get; set; }

            [Required]
            [StringLength(200)]
            public string Description { get; set; }

            [Required]
            [Range(1, 10000)]
            public int Amount { get; set; }

            [Required]
            [StringLength(maximumLength: 20, MinimumLength = 3)]
            public string UnitType { get; set; }

            [Precision(18, 2)]
            [Range(0.01, 100_000_000)]
            public decimal PricePerUnit { get; set; }

            [Required]
            public DateTime CreatedDate { get; set; }
            [Required]
            public DateTime ModifiedDate { get; set; }
        }
    }
}