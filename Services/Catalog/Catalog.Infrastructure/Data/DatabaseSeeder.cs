using Catalog.Core.Entities;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        public static async Task SeedAsync(IOptions<DatabaseSettings> options)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            var brands = db.GetCollection<ProductBrand>(settings.BrandCollectionName);
            var products = db.GetCollection<Product>(settings.ProductCollectionName);
            var types = db.GetCollection<ProductType>(settings.TypeCollectionName);

            var SeedBasePath = Path.Combine("Data", "SeedData");

            // Seed brands
            List<ProductBrand> brandList = new();
            if ((await brands.CountDocumentsAsync(_ => true) == 0))
            {
                var brandData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "brands.json"));
                brandList = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
                await brands.InsertManyAsync(brandList);
            }
            else
            {
                brandList = await brands.Find(_ => true).ToListAsync();
            }

            // Seed types
            List<ProductType> typeList = new();
            if ((await types.CountDocumentsAsync(_ => true) == 0))
            {
                var typesData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "types.json"));
                typeList = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                await types.InsertManyAsync(typeList);
            }
            else
            {
                typeList = await types.Find(_ => true).ToListAsync();
            }

            // Seed products 
            if ((await products.CountDocumentsAsync(_ => true) == 0))
            {
                var productData = await File.ReadAllTextAsync(Path.Combine(SeedBasePath, "products.json"));
                var productList = JsonSerializer.Deserialize<List<Product>>(productData);
                foreach (var product in productList)
                {
                    // Reset Id to let Mongo generate one;
                    product.Id = null;
                    // Default created Date if not set;
                    if (product.CreatedDate == default)
                        product.CreatedDate = DateTime.UtcNow;
                }
                await products.InsertManyAsync(productList);
            }
        }
    }
}
