using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using RealEstateAPI.Configuration;
using RealEstateAPI.Features.Properties.Models;
using RealEstateAPI.Features.Properties.DTOs;
using RealEstateAPI.Features.Shared.DTOs;

namespace RealEstateAPI.Features.Properties.Services;

/// <summary>
/// Service for managing properties in MongoDB
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly IMongoCollection<Property> _propertiesCollection;
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(
        IOptions<MongoDbSettings> mongoDbSettings,
        ILogger<PropertyService> logger)
    {
        _logger = logger;
        
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _propertiesCollection = mongoDatabase.GetCollection<Property>(mongoDbSettings.Value.PropertiesCollectionName);
        
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
            var indexKeysDefinition = Builders<Property>.IndexKeys
                .Ascending(x => x.Name)
                .Ascending(x => x.Address)
                .Ascending(x => x.Price);
            
            var indexOptions = new CreateIndexOptions { Background = true };
            _propertiesCollection.Indexes.CreateOne(new CreateIndexModel<Property>(indexKeysDefinition, indexOptions));
            
            _logger.LogInformation("MongoDB indexes created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating MongoDB indexes");
        }
    }

    public async Task<PagedResultDto<PropertyDto>> GetPropertiesAsync(PropertyFilterDto filter)
    {
        try
        {
            var filterBuilder = Builders<Property>.Filter;
            var filters = new List<FilterDefinition<Property>>
            {
                filterBuilder.Eq(x => x.IsActive, true)
            };

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                filters.Add(filterBuilder.Regex(x => x.Name, new BsonRegularExpression(filter.Name, "i")));
            }

            if (!string.IsNullOrWhiteSpace(filter.Address))
            {
                filters.Add(filterBuilder.Regex(x => x.Address, new BsonRegularExpression(filter.Address, "i")));
            }

            if (filter.MinPrice.HasValue)
            {
                filters.Add(filterBuilder.Gte(x => x.Price, filter.MinPrice.Value));
            }

            if (filter.MaxPrice.HasValue)
            {
                filters.Add(filterBuilder.Lte(x => x.Price, filter.MaxPrice.Value));
            }

            var combinedFilter = filterBuilder.And(filters);

            // Configurar ordenamiento
            var sortDefinition = GetSortDefinition(filter.SortBy, filter.SortDirection);

            // Contar total de elementos
            var totalCount = await _propertiesCollection.CountDocumentsAsync(combinedFilter);

            // Obtener elementos paginados
            var properties = await _propertiesCollection
                .Find(combinedFilter)
                .Sort(sortDefinition)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Limit(filter.PageSize)
                .ToListAsync();

            var propertyDtos = properties.Select(MapToDto).ToList();

            return new PagedResultDto<PropertyDto>
            {
                Items = propertyDtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving properties with filters");
            throw;
        }
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var property = await _propertiesCollection
                .Find(x => x.Id == id && x.IsActive)
                .FirstOrDefaultAsync();

            return property != null ? MapToDto(property) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving property by ID: {PropertyId}", id);
            throw;
        }
    }

    public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto)
    {
        try
        {
            var property = new Property
            {
                IdOwner = createPropertyDto.IdOwner,
                Name = createPropertyDto.Name,
                Address = createPropertyDto.Address,
                Price = createPropertyDto.Price,
                Images = await ProcessImageFilesAsync(createPropertyDto.Images),
                CodeInternal = createPropertyDto.CodeInternal,
                Year = createPropertyDto.Year,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _propertiesCollection.InsertOneAsync(property);
            
            _logger.LogInformation("Property created with ID: {PropertyId}", property.Id);
            
            return MapToDto(property);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property");
            throw;
        }
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(string id, UpdatePropertyDto updatePropertyDto)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var updateDefinition = Builders<Property>.Update
                .Set(x => x.Name, updatePropertyDto.Name)
                .Set(x => x.Address, updatePropertyDto.Address)
                .Set(x => x.Price, updatePropertyDto.Price)
                .Set(x => x.Images, await ProcessImageFilesAsync(updatePropertyDto.Images))
                .Set(x => x.CodeInternal, updatePropertyDto.CodeInternal)
                .Set(x => x.Year, updatePropertyDto.Year)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _propertiesCollection.UpdateOneAsync(
                x => x.Id == id && x.IsActive,
                updateDefinition);

            if (result.ModifiedCount == 0)
            {
                return null;
            }

            var updatedProperty = await _propertiesCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            _logger.LogInformation("Property updated: {PropertyId}", id);

            return updatedProperty != null ? MapToDto(updatedProperty) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property: {PropertyId}", id);
            throw;
        }
    }

    public async Task<bool> DeletePropertyAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }

            var updateDefinition = Builders<Property>.Update
                .Set(x => x.IsActive, false)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _propertiesCollection.UpdateOneAsync(
                x => x.Id == id && x.IsActive,
                updateDefinition);

            var deleted = result.ModifiedCount > 0;
            
            if (deleted)
            {
                _logger.LogInformation("Property deleted (soft delete): {PropertyId}", id);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property: {PropertyId}", id);
            throw;
        }
    }

    public async Task<PropertyDto?> ChangePricePropertyAsync(string id, decimal newPrice)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var updateDefinition = Builders<Property>.Update
                .Set(x => x.Price, newPrice)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _propertiesCollection.UpdateOneAsync(
                x => x.Id == id && x.IsActive,
                updateDefinition);

            if (result.ModifiedCount == 0)
            {
                return null;
            }

            var updatedProperty = await _propertiesCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            _logger.LogInformation("Property price updated: {PropertyId} - New Price: {NewPrice}", id, newPrice);

            return updatedProperty != null ? MapToDto(updatedProperty) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating property price: {PropertyId}", id);
            throw;
        }
    }

    /// <summary>
    /// Gets the sort definition based on parameters
    /// </summary>
    private static SortDefinition<Property> GetSortDefinition(string sortBy, string sortDirection)
    {
        var sortBuilder = Builders<Property>.Sort;
        var isAscending = sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLower() switch
        {
            "name" => isAscending ? sortBuilder.Ascending(x => x.Name) : sortBuilder.Descending(x => x.Name),
            "address" => isAscending ? sortBuilder.Ascending(x => x.Address) : sortBuilder.Descending(x => x.Address),
            "price" => isAscending ? sortBuilder.Ascending(x => x.Price) : sortBuilder.Descending(x => x.Price),
            "createdat" => isAscending ? sortBuilder.Ascending(x => x.CreatedAt) : sortBuilder.Descending(x => x.CreatedAt),
            _ => sortBuilder.Descending(x => x.CreatedAt)
        };
    }

    /// <summary>
    /// Processes uploaded image files and returns their URLs/paths
    /// </summary>
    /// <param name="imageFiles">Array of uploaded image files</param>
    /// <returns>List of processed image URLs/paths</returns>
    private async Task<List<string>> ProcessImageFilesAsync(IFormFile[]? imageFiles)
    {
        var imageUrls = new List<string>();
        
        if (imageFiles == null || imageFiles.Length == 0)
        {
            return imageUrls;
        }

        try
        {
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "properties");
            Directory.CreateDirectory(uploadsPath);

            foreach (var file in imageFiles)
            {
                if (file.Length > 0 && IsValidImageFile(file))
                {
                    // Generate unique filename
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Add relative URL to list
                    imageUrls.Add($"/uploads/properties/{fileName}");
                    
                    _logger.LogInformation("Image saved: {FileName}", fileName);
                }
                else
                {
                    _logger.LogWarning("Invalid image file rejected: {FileName}", file.FileName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image files");
            throw;
        }

        return imageUrls;
    }

    /// <summary>
    /// Validates if the uploaded file is a valid image
    /// </summary>
    /// <param name="file">File to validate</param>
    /// <returns>True if valid image file</returns>
    private static bool IsValidImageFile(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var contentType = file.ContentType.ToLowerInvariant();
        
        return allowedExtensions.Contains(extension) && allowedContentTypes.Contains(contentType);
    }

    /// <summary>
    /// Maps a Property entity to PropertyDto
    /// </summary>
    private static PropertyDto MapToDto(Property property)
    {
        return new PropertyDto
        {
            Id = property.Id ?? string.Empty,
            IdOwner = property.IdOwner,
            Name = property.Name,
            Address = property.Address,
            Price = property.Price,
            Images = property.Images,
            CodeInternal = property.CodeInternal,
            Year = property.Year,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt,
            IsActive = property.IsActive
        };
    }
}
