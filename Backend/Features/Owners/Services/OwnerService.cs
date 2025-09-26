using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using RealEstateAPI.Configuration;
using RealEstateAPI.Features.Owners.Models;
using RealEstateAPI.Features.Owners.DTOs;
using RealEstateAPI.Features.Shared.DTOs;
using RealEstateAPI.Features.Shared.Services;

namespace RealEstateAPI.Features.Owners.Services;

/// <summary>
/// Service for managing owners in MongoDB
/// </summary>
public class OwnerService : IOwnerService
{
    private readonly IMongoCollection<Owner> _ownersCollection;
    private readonly ILogger<OwnerService> _logger;
    private readonly IImageService _imageService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OwnerService(
        IOptions<MongoDbSettings> mongoDbSettings,
        ILogger<OwnerService> logger,
        IImageService imageService,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _imageService = imageService;
        _httpContextAccessor = httpContextAccessor;
        
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _ownersCollection = mongoDatabase.GetCollection<Owner>(mongoDbSettings.Value.OwnersCollectionName);
        
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
            var indexKeysDefinition = Builders<Owner>.IndexKeys
                .Ascending(x => x.Name)
                .Ascending(x => x.IsActive);
            
            var indexOptions = new CreateIndexOptions { Background = true };
            _ownersCollection.Indexes.CreateOne(new CreateIndexModel<Owner>(indexKeysDefinition, indexOptions));
            
            _logger.LogInformation("MongoDB indexes for Owners created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating MongoDB indexes for Owners");
        }
    }

    public async Task<PagedResultDto<OwnerDto>> GetOwnersAsync(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var filter = Builders<Owner>.Filter.Eq(x => x.IsActive, true);
            var sort = Builders<Owner>.Sort.Descending(x => x.CreatedAt);

            var totalCount = await _ownersCollection.CountDocumentsAsync(filter);

            var owners = await _ownersCollection
                .Find(filter)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            var ownerDtos = owners.Select(MapToDto).ToList();

            return new PagedResultDto<OwnerDto>
            {
                Items = ownerDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving owners");
            throw;
        }
    }

    public async Task<OwnerDto?> GetOwnerByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var owner = await _ownersCollection
                .Find(x => x.Id == id && x.IsActive)
                .FirstOrDefaultAsync();

            return owner != null ? MapToDto(owner) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving owner by ID: {OwnerId}", id);
            throw;
        }
    }

    public async Task<OwnerDto> CreateOwnerAsync(CreateOwnerDto createOwnerDto, IFormFile? photo = null)
    {
        try
        {
            string photoUrl = string.Empty;

            // Process photo file if provided
            if (photo != null && photo.Length > 0)
            {
                if (_imageService.IsValidImageFile(photo))
                {
                    var photoUrls = await _imageService.ProcessImageFilesAsync(new[] { photo }, "owners");
                    photoUrl = photoUrls.FirstOrDefault() ?? string.Empty;
                    
                    _logger.LogInformation("Owner photo processed successfully: {PhotoUrl}", photoUrl);
                }
                else
                {
                    _logger.LogWarning("Invalid photo file provided for owner creation: {FileName}", photo.FileName);
                    throw new ArgumentException("Invalid photo file. Please provide a valid image file (jpg, png, gif, webp) under 5MB.");
                }
            }

            var owner = new Owner
            {
                Name = createOwnerDto.Name,
                Address = createOwnerDto.Address,
                Photo = photoUrl,
                Birthday = createOwnerDto.Birthday,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _ownersCollection.InsertOneAsync(owner);
            
            _logger.LogInformation("Owner created with ID: {OwnerId}", owner.Id);
            
            return MapToDto(owner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating owner");
            throw;
        }
    }

    public async Task<OwnerDto?> UpdateOwnerAsync(string id, CreateOwnerDto updateOwnerDto)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var updateDefinition = Builders<Owner>.Update
                .Set(x => x.Name, updateOwnerDto.Name)
                .Set(x => x.Address, updateOwnerDto.Address)
                .Set(x => x.Birthday, updateOwnerDto.Birthday)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _ownersCollection.UpdateOneAsync(
                x => x.Id == id && x.IsActive,
                updateDefinition);

            if (result.ModifiedCount == 0)
            {
                return null;
            }

            var updatedOwner = await _ownersCollection
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync();

            _logger.LogInformation("Owner updated: {OwnerId}", id);

            return updatedOwner != null ? MapToDto(updatedOwner) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating owner: {OwnerId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteOwnerAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }

            var updateDefinition = Builders<Owner>.Update
                .Set(x => x.IsActive, false)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _ownersCollection.UpdateOneAsync(
                x => x.Id == id && x.IsActive,
                updateDefinition);

            var deleted = result.ModifiedCount > 0;
            
            if (deleted)
            {
                _logger.LogInformation("Owner deleted (soft delete): {OwnerId}", id);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting owner: {OwnerId}", id);
            throw;
        }
    }

    /// <summary>
    /// Maps an Owner entity to OwnerDto
    /// </summary>
    private OwnerDto MapToDto(Owner owner)
    {
        return new OwnerDto
        {
            Id = owner.Id ?? string.Empty,
            Name = owner.Name,
            Address = owner.Address,
            Photo = GetFullImageUrl(owner.Photo),
            Birthday = owner.Birthday,
            CreatedAt = owner.CreatedAt,
            UpdatedAt = owner.UpdatedAt,
            IsActive = owner.IsActive
        };
    }

    /// <summary>
    /// Converts a relative image path to a full URL
    /// </summary>
    private string GetFullImageUrl(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            _logger.LogDebug("GetFullImageUrl: relativePath is null or empty");
            return string.Empty;
        }

        // If it's already a full URL, return as is
        if (relativePath.StartsWith("http://") || relativePath.StartsWith("https://"))
        {
            _logger.LogDebug("GetFullImageUrl: relativePath is already a full URL: {RelativePath}", relativePath);
            return relativePath;
        }

        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request != null)
            {
                var request = httpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                var fullUrl = $"{baseUrl}{relativePath}";
                _logger.LogDebug("GetFullImageUrl: Generated full URL: {FullUrl} from base: {BaseUrl} and path: {RelativePath}", 
                    fullUrl, baseUrl, relativePath);
                return fullUrl;
            }
            else
            {
                _logger.LogWarning("GetFullImageUrl: HttpContext or Request is null");
                // Fallback to localhost for development
                var fallbackUrl = $"http://localhost:5000{relativePath}";
                _logger.LogDebug("GetFullImageUrl: Using fallback URL: {FallbackUrl}", fallbackUrl);
                return fallbackUrl;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error generating full URL for image: {RelativePath}", relativePath);
            // Return relative path as fallback
            return relativePath;
        }
    }
}
