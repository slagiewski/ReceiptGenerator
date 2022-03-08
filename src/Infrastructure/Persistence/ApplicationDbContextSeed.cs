using Microsoft.AspNetCore.Identity;
using ReceiptGenerator.Domain.Entities;
using ReceiptGenerator.Domain.ValueObjects;
using ReceiptGenerator.Infrastructure.Identity;

namespace ReceiptGenerator.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var administratorRole = new IdentityRole("Administrator");

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }

            var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

            if (userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await userManager.CreateAsync(administrator, "Administrator1!");
                await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            if (context.Receipts.Any())
            {
                return;   // DB has been seeded
            }

            var receiptSellers = new ApplicationDbContext.ReceiptSeller[]
            {
                new ApplicationDbContext.ReceiptSeller { FirstName="Sebastian", LastName="Łągiewski", Street="Lipowa 66", ZipCode="50-431", BankAccountNo = "91126022020000000302348731", City="Warka", CreatedDate=DateTime.Now, ModifiedDate = DateTime.Now, }
            };

            var buyers = new ApplicationDbContext.Buyer[]
            {
                new ApplicationDbContext.Buyer { Name="Dandydat sp. z o.o.", AddressDetails="Lok 13", Nip="1111111", Regon=null, Street="Klonowa 15", ZipCode="96-100", City="Skierniewice", CreatedDate=DateTime.Now, ModifiedDate = DateTime.Now }
            };

            context.ReceiptSellers.AddRange(receiptSellers);
            context.Buyers.AddRange(buyers);

            context.SaveChanges();

            var receipts = new ApplicationDbContext.Receipt[]
            {
                new ApplicationDbContext.Receipt { ReceiptNo="04/01/2022", BuyerId=buyers.First().Id, SellerId=receiptSellers.First().Id, TotalAmount=21.50m, ReceiptCity="Przygłów", ReceiptDate=DateTime.Now, AdditionalDescription="Płatne do 09.09.2022r.", CreatedDate=DateTime.Now, ModifiedDate = DateTime.Now },
            };

            context.Receipts.AddRange(receipts);
            context.SaveChanges();

            var receiptItems = new ApplicationDbContext.ReceiptItem[]
            {
                new ApplicationDbContext.ReceiptItem { Amount = 3, Description="Nocleg", PricePerUnit=21.50m, ReceiptId=receipts.First().Id, UnitType="os.", CreatedDate=DateTime.Now, ModifiedDate = DateTime.Now }
            };

            context.ReceiptItems.AddRange(receiptItems);
            context.SaveChanges();
        }
    }
}