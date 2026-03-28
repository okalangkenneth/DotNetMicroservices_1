using Catalog.API.Entities;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Catalog.API.Data
{
    public class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            bool existProduct = productCollection.Find(p => true).Any();
            if (!existProduct)
            {
                productCollection.InsertManyAsync(GetPreconfiguredProducts());
            }
        }

        private static IEnumerable<Product> GetPreconfiguredProducts()
        {
            return new List<Product>()
{
    new Product()
    {
        Name = "Apple iPhone 13",
        Category = "Electronics",
        Summary = "Latest Apple iPhone model",
        Description = "Apple iPhone 13 with 128GB storage, 5G support, and A15 Bionic chip",
        ImageFile = "iphone13.png",
        Price = 699
    },
    new Product()
    {
        Name = "Samsung Galaxy S21",
        Category = "Electronics",
        Summary = "Latest Samsung Galaxy model",
        Description = "Samsung Galaxy S21 with 128GB storage, 5G support, and Snapdragon 888",
        ImageFile = "galaxys21.png",
        Price = 799
    },
    new Product()
    {
        Name = "Dell XPS 15",
        Category = "Computers",
        Summary = "High-performance laptop from Dell",
        Description = "Dell XPS 15 with Intel Core i7, 16GB RAM, and 512GB SSD",
        ImageFile = "dellxps15.png",
        Price = 1699
    },
    new Product()
    {
        Name = "Sony WH-1000XM4",
        Category = "Audio",
        Summary = "Top-rated noise-cancelling headphones",
        Description = "Sony WH-1000XM4 wireless noise-cancelling headphones with up to 30 hours of battery life",
        ImageFile = "sonywh1000xm4.png",
        Price = 349
    }
};

        }
    }
}

