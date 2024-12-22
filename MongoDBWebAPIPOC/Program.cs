using MongoDBWebAPIPOC.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDBWebAPIPOC;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDBSettings"));

builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = s.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var mongoSettings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoDbContext(mongoSettings.ConnectionString, mongoSettings.DatabaseName);
});

builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<productService>();
builder.Services.AddScoped<ProductController>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ObjectIdJsonConverter()); // convert from objectid json to objectid
});


// Add Swagger services
builder.Services.AddEndpointsApiExplorer(); // Required for minimal APIs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MongoDB Web API POC", Version = "v1" });
});

var app = builder.Build();

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MongoDB Web API POC v1");
    });
}

app.MapControllers();
app.Run();
