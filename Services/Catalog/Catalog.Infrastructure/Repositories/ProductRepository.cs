using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specifications;
using Catalog.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<ProductBrand> _brands;
        private readonly IMongoCollection<ProductType> _types;
        private readonly IMongoCollection<Product> _products;
        public ProductRepository(IOptions<DatabaseSettings> options)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _brands = db.GetCollection<ProductBrand>(settings.BrandCollectionName);
            _types = db.GetCollection<ProductType>(settings.TypeCollectionName);
            _products = db.GetCollection<Product>(settings.ProductCollectionName);

        }
        async Task<Product> IProductRepository.CreateProduct(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        async Task<bool> IProductRepository.DeleteProduct(string productId)
        {
            var deletedProduct = await _products.DeleteOneAsync(x => x.Id == productId);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }

        async Task<IEnumerable<Product>> IProductRepository.GetAllAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        async Task<ProductBrand> IProductRepository.GetBrandByIdAsync(string brandId)
        {
            return await _brands.Find(x => x.Id == brandId).FirstOrDefaultAsync();
        }

        async Task<Product> IProductRepository.GetProduct(string productId)
        {
            return await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
        }

        async Task<Pagination<Product>> IProductRepository.GetProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;
            if (!string.IsNullOrEmpty(catalogSpecParams.Search))
            {
                filter &= builder.Where(p => p.Name.ToLower().Contains(catalogSpecParams.Search.ToLower()));
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.BrandId))
            {
                filter &= builder.Eq(p => p.Brand.Id, catalogSpecParams.BrandId);
            }
            if (!string.IsNullOrEmpty(catalogSpecParams.TypeId))
            {
                filter &= builder.Eq(p => p.Type.Id, catalogSpecParams.TypeId);
            }

            var totalItems = await _products.CountDocumentsAsync(filter);
            var data = await ApplyDataFilters(catalogSpecParams, filter);
            return new Pagination<Product>(
                catalogSpecParams.PageIndex,
                catalogSpecParams.PageSize,
                (int)totalItems,
                data
                );
        }

        async Task<IEnumerable<Product>> IProductRepository.GetProductsByBrand(string name)
        {
            return await _products.Find(p => p.Brand.Name.ToLower() == name.ToLower()).ToListAsync();
        }

        async Task<IEnumerable<Product>> IProductRepository.GetProductsByName(string name)
        {
            var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression($".*{name}.*", "i"));
            return await _products.Find(filter).ToListAsync();
        }

        async Task<ProductType> IProductRepository.GetTypeByIdAsync(string typeId)
        {
            return await _types.Find(t => t.Id == typeId).FirstOrDefaultAsync();
        }

        async Task<bool> IProductRepository.UpdateProduct(Product product)
        {
            var updatedProduct = await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }

        private async Task<IReadOnlyCollection<Product>> ApplyDataFilters(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortDef = Builders<Product>.Sort.Ascending(p => p.Name);
            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                sortDef = catalogSpecParams.Sort switch
                {
                    "priceAsc" => Builders<Product>.Sort.Ascending(p => p.Price),
                    "priceDesc" => Builders<Product>.Sort.Descending(p => p.Price),
                    _ => Builders<Product>.Sort.Ascending(p => p.Name)
                };
            }
            return await _products
                .Find(filter)
                .Sort(sortDef)
                .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                .Limit(catalogSpecParams.PageSize)
                .ToListAsync();
        }
    }
}
