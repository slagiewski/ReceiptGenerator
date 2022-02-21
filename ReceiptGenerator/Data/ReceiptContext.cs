using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ReceiptGenerator.Data
{
    public class ReceiptContext : IdentityDbContext
    {
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptItem> ReceiptItems { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<ReceiptSeller> ReceiptSellers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public ReceiptContext(DbContextOptions<ReceiptContext> options)
            : base(options)
        {
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