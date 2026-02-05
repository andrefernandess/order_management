using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Interfaces.Repositories;

namespace OrderManagement.Infrastructure.MongoDB;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _collection;

    public ProductRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Product>("Products");

        // Cria índices
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var indexOptions = new CreateIndexOptions { Background = true };

        var categoryIndex = new CreateIndexModel<Product>(
            Builders<Product>.IndexKeys.Ascending(p => p.Category),
            indexOptions);

        var nameIndex = new CreateIndexModel<Product>(
            Builders<Product>.IndexKeys.Text(p => p.Name).Text(p => p.Description),
            indexOptions);

        var activeIndex = new CreateIndexModel<Product>(
            Builders<Product>.IndexKeys.Ascending(p => p.IsActive),
            indexOptions);

        _collection.Indexes.CreateMany(new[] { categoryIndex, nameIndex, activeIndex });
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out _))
            return null;

        return await _collection
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        return await _collection
            .Find(_ => true)
            .SortBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(string category, CancellationToken ct = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Category, category);

        return await _collection
            .Find(filter)
            .SortBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken ct = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.IsActive, true);

        return await _collection
            .Find(filter)
            .SortBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(string searchTerm, CancellationToken ct = default)
    {
        var filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(searchTerm, "i")),
            Builders<Product>.Filter.Regex(p => p.Description, new BsonRegularExpression(searchTerm, "i")),
            Builders<Product>.Filter.AnyEq(p => p.Tags, searchTerm.ToLower())
        );

        return await _collection
            .Find(filter)
            .SortBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(product, cancellationToken: ct);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        await _collection.ReplaceOneAsync(
            p => p.Id == product.Id,
            product,
            cancellationToken: ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        await _collection.DeleteOneAsync(p => p.Id == id, ct);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out _))
            return false;

        return await _collection
            .Find(p => p.Id == id)
            .AnyAsync(ct);
    }
}
