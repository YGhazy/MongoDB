using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBWebAPIPOC
{
    public class productService
    {
        private readonly MongoDbContext _context;

        public productService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Find(_ => true).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            return await _context.Products.Find(model => model.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Product model)
        {
            await _context.Products.InsertOneAsync(model);
        }

        public async Task UpdateAsync(string id, Product model)
        {
            var objectId = new ObjectId(id);
            await _context.Products.ReplaceOneAsync(m => m.Id == objectId, model);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            await _context.Products.DeleteOneAsync(m => m.Id == objectId);
        }

        public async Task<List<Product>> GetPagedProductsAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Find(_ => true) 
                .Skip((pageNumber - 1) * pageSize) 
                .Limit(pageSize) 
                .ToListAsync();
        }


        public async Task InsertManyAsync(List<Product> products)
        {
            var bulkOps = new List<WriteModel<Product>>();

            foreach (var product in products)
            {
                bulkOps.Add(new InsertOneModel<Product>(product));
            }

            await _context.Products.BulkWriteAsync(bulkOps);
        }
        public async Task<List<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var filter = Builders<Product>.Filter.Gte(p => p.Price, minPrice) &
                         Builders<Product>.Filter.Lte(p => p.Price, maxPrice);

            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task<List<Product>> GetByNameAsync(string name)
        {
            var filter = Builders<Product>.Filter.Regex(p => p.Name, 
                new BsonRegularExpression(name, "i")); // case-insensitive search
            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task UpdateMultipleProductsAsync(List<Product> products)
        {
            using (var session = await _context.Client.StartSessionAsync())
            {
                session.StartTransaction();

                try
                {
                    foreach (var product in products)
                    {
                        var objectId = new ObjectId(product.Id.ToString());
                        var update = Builders<Product>.Update.Set(p => p.Price, product.Price);
                        await _context.Products.UpdateOneAsync(session, p => p.Id == objectId, update);
                    }

                    await session.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await session.AbortTransactionAsync();
                    throw;
                }
            }
        }

    }
}
