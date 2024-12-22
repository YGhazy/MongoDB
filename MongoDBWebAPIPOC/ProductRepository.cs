    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;

namespace MongoDBWebAPIPOC
{
    public class ProductRepository
    {
        private readonly IMongoCollection<Product> _collection;

        public ProductRepository(MongoDbContext dbContext)
        {
            _collection = dbContext.GetCollection<Product>("products");
        }

        public async Task CreateAsync(Product product)
        {
            await _collection.InsertOneAsync(product);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

    }
}
