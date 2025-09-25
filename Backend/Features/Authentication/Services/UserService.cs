using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using RealEstateAPI.Configuration;
using RealEstateAPI.Features.Authentication.Models;

namespace RealEstateAPI.Features.Authentication.Services;

/// <summary>
/// Service for managing users in MongoDB
/// </summary>
public class UserService : IUserService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IOptions<MongoDbSettings> mongoDbSettings,
        ILogger<UserService> logger)
    {
        _logger = logger;
        
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(mongoDbSettings.Value.UsersCollectionName);
        
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
            // Unique index on email
            var emailIndexKeysDefinition = Builders<User>.IndexKeys.Ascending(x => x.Email);
            var emailIndexOptions = new CreateIndexOptions { Background = true, Unique = true };
            _usersCollection.Indexes.CreateOne(new CreateIndexModel<User>(emailIndexKeysDefinition, emailIndexOptions));

            // Index on refresh token
            var refreshTokenIndexKeysDefinition = Builders<User>.IndexKeys.Ascending(x => x.RefreshToken);
            var refreshTokenIndexOptions = new CreateIndexOptions { Background = true };
            _usersCollection.Indexes.CreateOne(new CreateIndexModel<User>(refreshTokenIndexKeysDefinition, refreshTokenIndexOptions));

            // Compound index on isActive and role
            var compoundIndexKeysDefinition = Builders<User>.IndexKeys
                .Ascending(x => x.IsActive)
                .Ascending(x => x.Role);
            var compoundIndexOptions = new CreateIndexOptions { Background = true };
            _usersCollection.Indexes.CreateOne(new CreateIndexModel<User>(compoundIndexKeysDefinition, compoundIndexOptions));
            
            _logger.LogInformation("MongoDB indexes for Users created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating MongoDB indexes for Users");
        }
    }

    public async Task<User> CreateUserAsync(User user)
    {
        try
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            
            await _usersCollection.InsertOneAsync(user);
            
            _logger.LogInformation("User created with ID: {UserId} and Email: {Email}", user.Id, user.Email);
            
            return user;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            _logger.LogWarning("Attempted to create user with duplicate email: {Email}", user.Email);
            throw new InvalidOperationException("A user with this email already exists");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email: {Email}", user.Email);
            throw;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var user = await _usersCollection
                .Find(x => x.Email.ToLower() == email.ToLower() && x.IsActive)
                .FirstOrDefaultAsync();

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        try
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var user = await _usersCollection
                .Find(x => x.Id == id && x.IsActive)
                .FirstOrDefaultAsync();

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {UserId}", id);
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        try
        {
            user.UpdatedAt = DateTime.UtcNow;
            
            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            var result = await _usersCollection.ReplaceOneAsync(filter, user);
            
            if (result.MatchedCount == 0)
            {
                throw new InvalidOperationException("User not found");
            }
            
            _logger.LogInformation("User updated with ID: {UserId}", user.Id);
            
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime refreshTokenExpiry)
    {
        try
        {
            if (!ObjectId.TryParse(userId, out _))
            {
                return false;
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update
                .Set(x => x.RefreshToken, refreshToken)
                .Set(x => x.RefreshTokenExpiry, refreshTokenExpiry)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(filter, update);
            
            return result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating refresh token for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            var user = await _usersCollection
                .Find(x => x.RefreshToken == refreshToken && x.IsActive)
                .FirstOrDefaultAsync();

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by refresh token");
            throw;
        }
    }

    public async Task<bool> UpdateLastLoginAsync(string userId)
    {
        try
        {
            if (!ObjectId.TryParse(userId, out _))
            {
                return false;
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update
                .Set(x => x.LastLogin, DateTime.UtcNow)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(filter, update);
            
            return result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last login for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> IncrementFailedLoginAttemptsAsync(string userId)
    {
        try
        {
            if (!ObjectId.TryParse(userId, out _))
            {
                return false;
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update
                .Inc(x => x.FailedLoginAttempts, 1)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(filter, update);
            
            return result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing failed login attempts for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ResetFailedLoginAttemptsAsync(string userId)
    {
        try
        {
            if (!ObjectId.TryParse(userId, out _))
            {
                return false;
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update
                .Set(x => x.FailedLoginAttempts, 0)
                .Unset(x => x.LockoutEnd)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(filter, update);
            
            return result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting failed login attempts for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> LockUserAccountAsync(string userId, DateTime lockoutEnd)
    {
        try
        {
            if (!ObjectId.TryParse(userId, out _))
            {
                return false;
            }

            var filter = Builders<User>.Filter.Eq(x => x.Id, userId);
            var update = Builders<User>.Update
                .Set(x => x.LockoutEnd, lockoutEnd)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _usersCollection.UpdateOneAsync(filter, update);
            
            return result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking user account for user ID: {UserId}", userId);
            throw;
        }
    }
}
