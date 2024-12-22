using MongoDB.Driver;

public class MongoDbContext
{
	private readonly IMongoDatabase _database;
    private readonly IMongoClient _client;

    public MongoDbContext(string connectionString, string databaseName)
	{
        _client = new MongoClient(connectionString);
		_database = _client.GetDatabase(databaseName);
	}

    public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
    // each collection

    public IMongoClient Client => _client;


    //Generic Method for any collection
    public IMongoCollection<T> GetCollection<T>(string collectionName)
	{
		return _database.GetCollection<T>(collectionName);
	}
}