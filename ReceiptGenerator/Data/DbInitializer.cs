namespace ReceiptGenerator.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ReceiptContext context)
        {
            context.Database.EnsureCreated();

            if (context.Receipts.Any())
            {
                return;   // DB has been seeded
            }

            var receiptSellers = new ReceiptSeller[]
            {
                new ReceiptSeller { FirstName="Sebastian", LastName="Łągiewski", Street="Lipowa 66", ZipCode="50-431", BankAccountNo = "91126022020000000302348731", City="Warka", CreatedDate=DateTime.Now, ModifiedDate = DateTime.Now, }
            };

            var buyers = new Buyer[]
            {
                new Buyer { Name="Dandydat sp. z o.o.", AddressDetails="Lok 13", Nip="1111111", Regon=null, Street="Klonowa 15", ZipCode="96-100", City="Skierniewice", CreatedDate=DateTime.Now, ModifiedDate = DateTime.Now }
            };

            context.ReceiptSellers.AddRange(receiptSellers);
            context.Buyers.AddRange(buyers);

            context.SaveChanges();

            var receipts = new Receipt[]
            {
                new Receipt { ReceiptNo="04/01/2022", BuyerId=buyers.First().Id, SellerId=receiptSellers.First().Id, TotalAmount=21.50m, ReceiptCity="Przygłów", ReceiptDate=DateTime.Now, AdditionalDescription="Płatne do 09.09.2022r.", CreatedDate=DateTime.Now, ModifiedDate = DateTime.Now },
            };

            context.Receipts.AddRange(receipts);
            context.SaveChanges();

            var receiptItems = new ReceiptItem[]
            {
                new ReceiptItem { Amount = 3, Description="Nocleg", PricePerUnit=21.50m, ReceiptId=receipts.First().Id, UnitType="os.", CreatedDate=DateTime.Now, ModifiedDate = DateTime.Now }
            };

            context.ReceiptItems.AddRange(receiptItems);
            context.SaveChanges();
        }

    }
}
