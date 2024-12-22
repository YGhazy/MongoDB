
using MongoDB.Bson;

public class Product
{
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId(); // auto generate Id
    public string Name { get; set; }
    public decimal Price { get; set; }
}