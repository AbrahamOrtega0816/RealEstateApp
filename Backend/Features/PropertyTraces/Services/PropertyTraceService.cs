using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using RealEstateAPI.Configuration;
using RealEstateAPI.Features.PropertyTraces.Models;
using RealEstateAPI.Features.PropertyTraces.DTOs;

namespace RealEstateAPI.Features.PropertyTraces.Services;

/// <summary>
/// Service for managing property traces in MongoDB
/// </summary>
public class PropertyTraceService : IPropertyTraceService
{
    private readonly IMongoCollection<PropertyTrace> _propertyTracesCollection;
    private readonly ILogger<PropertyTraceService> _logger;

    public PropertyTraceService(
        IOptions<MongoDbSettings> mongoDbSettings,
        ILogger<PropertyTraceService> logger)
    {
        _logger = logger;
        
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _propertyTracesCollection = mongoDatabase.GetCollection<PropertyTrace>(mongoDbSettings.Value.PropertyTracesCollectionName);
        
        // Create indexes to optimize queries
        CreateIndexes();
    }

    /// <summary>
    /// Creates indexes in MongoDB to optimize queries
    /// </summary>
    private void CreateIndexes()
    {
        try
        {
            var indexKeysDefinition = Builders<PropertyTrace>.IndexKeys
                .Ascending(x => x.IdProperty)
                .Descending(x => x.DateSale);
            
            var indexOptions = new CreateIndexOptions { Background = true };
            _propertyTracesCollection.Indexes.CreateOne(new CreateIndexModel<PropertyTrace>(indexKeysDefinition, indexOptions));
            
            _logger.LogInformation("MongoDB indexes for PropertyTraces created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating MongoDB indexes for PropertyTraces");
        }
    }

    public async Task<IEnumerable<PropertyTraceDto>> GetTracesByPropertyIdAsync(string propertyId)
    {
        try
        {
            if (!ObjectId.TryParse(propertyId, out _))
            {
                return Enumerable.Empty<PropertyTraceDto>();
            }

            var traces = await _propertyTracesCollection
                .Find(x => x.IdProperty == propertyId)
                .SortByDescending(x => x.DateSale)
                .ToListAsync();

            return traces.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving traces for property: {PropertyId}", propertyId);
            throw;
        }
    }

    public async Task<PropertyTraceDto?> GetTraceByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var trace = await _propertyTracesCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            return trace != null ? MapToDto(trace) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property trace by ID: {TraceId}", id);
            throw;
        }
    }

    public async Task<PropertyTraceDto> CreateTraceAsync(CreatePropertyTraceDto createTraceDto)
    {
        try
        {
            var propertyTrace = new PropertyTrace
            {
                DateSale = createTraceDto.DateSale,
                Name = createTraceDto.Name,
                Value = createTraceDto.Value,
                Tax = createTraceDto.Tax,
                IdProperty = createTraceDto.IdProperty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _propertyTracesCollection.InsertOneAsync(propertyTrace);
            
            _logger.LogInformation("Property trace created with ID: {TraceId}", propertyTrace.Id);
            
            return MapToDto(propertyTrace);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property trace");
            throw;
        }
    }

    public async Task<PropertyTraceDto?> UpdateTraceAsync(string id, CreatePropertyTraceDto updateTraceDto)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var updateDefinition = Builders<PropertyTrace>.Update
                .Set(x => x.DateSale, updateTraceDto.DateSale)
                .Set(x => x.Name, updateTraceDto.Name)
                .Set(x => x.Value, updateTraceDto.Value)
                .Set(x => x.Tax, updateTraceDto.Tax)
                .Set(x => x.IdProperty, updateTraceDto.IdProperty)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _propertyTracesCollection.UpdateOneAsync(
                x => x.Id == id,
                updateDefinition);

            if (result.ModifiedCount == 0)
            {
                return null;
            }

            var updatedTrace = await _propertyTracesCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            _logger.LogInformation("Property trace updated: {TraceId}", id);

            return updatedTrace != null ? MapToDto(updatedTrace) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property trace: {TraceId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTraceAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }

            var result = await _propertyTracesCollection.DeleteOneAsync(x => x.Id == id);

            var deleted = result.DeletedCount > 0;
            
            if (deleted)
            {
                _logger.LogInformation("Property trace deleted: {TraceId}", id);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property trace: {TraceId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<PropertyTraceDto>> GetTracesByDateRangeAsync(string propertyId, DateTime startDate, DateTime endDate)
    {
        try
        {
            if (!ObjectId.TryParse(propertyId, out _))
            {
                return Enumerable.Empty<PropertyTraceDto>();
            }

            var filterBuilder = Builders<PropertyTrace>.Filter;
            var filter = filterBuilder.And(
                filterBuilder.Eq(x => x.IdProperty, propertyId),
                filterBuilder.Gte(x => x.DateSale, startDate),
                filterBuilder.Lte(x => x.DateSale, endDate)
            );

            var traces = await _propertyTracesCollection
                .Find(filter)
                .SortByDescending(x => x.DateSale)
                .ToListAsync();

            return traces.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving traces for property: {PropertyId} between {StartDate} and {EndDate}", 
                propertyId, startDate, endDate);
            throw;
        }
    }

    /// <summary>
    /// Maps a PropertyTrace entity to PropertyTraceDto
    /// </summary>
    private static PropertyTraceDto MapToDto(PropertyTrace propertyTrace)
    {
        return new PropertyTraceDto
        {
            Id = propertyTrace.Id ?? string.Empty,
            DateSale = propertyTrace.DateSale,
            Name = propertyTrace.Name,
            Value = propertyTrace.Value,
            Tax = propertyTrace.Tax,
            IdProperty = propertyTrace.IdProperty,
            CreatedAt = propertyTrace.CreatedAt,
            UpdatedAt = propertyTrace.UpdatedAt
        };
    }
}
