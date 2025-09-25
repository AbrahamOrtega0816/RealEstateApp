using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealEstateAPI.Configuration;

namespace RealEstateAPI.Features.Shared.Services;

/// <summary>
/// Service for MongoDB startup verification and health checks
/// </summary>
public class MongoDbStartupService : IMongoDbStartupService
{
    private readonly MongoDbSettings _mongoSettings;
    private readonly ILogger<MongoDbStartupService> _logger;
    private readonly IWebHostEnvironment _environment;

    public MongoDbStartupService(
        IOptions<MongoDbSettings> mongoSettings,
        ILogger<MongoDbStartupService> logger,
        IWebHostEnvironment environment)
    {
        _mongoSettings = mongoSettings.Value;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Verifies MongoDB connection during application startup
    /// </summary>
    public async Task VerifyConnectionAsync()
    {
        try
        {
            _logger.LogInformation("🔄 Starting MongoDB connection verification...");

            // Verify environment variables first
            ValidateEnvironmentVariables();

            // Verify basic configuration
            ValidateConfiguration();

            // Create client and connect
            var client = new MongoClient(_mongoSettings.ConnectionString);
            var database = client.GetDatabase(_mongoSettings.DatabaseName);

            // Perform ping to verify connectivity
            _logger.LogInformation("🔍 Testing MongoDB connection...");
            await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(
                new MongoDB.Bson.BsonDocument("ping", 1));

            // Get server information
            var serverStatus = await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(
                new MongoDB.Bson.BsonDocument("serverStatus", 1));

            LogConnectionSuccess(serverStatus);
            LogCollectionConfiguration();

            _logger.LogInformation("🚀 Database system ready to use!");
        }
        catch (MongoException mongoEx)
        {
            HandleMongoException(mongoEx);
        }
        catch (Exception ex)
        {
            HandleGeneralException(ex);
        }
    }

    /// <summary>
    /// Gets MongoDB connection information
    /// </summary>
    public async Task<object> GetConnectionInfoAsync()
    {
        try
        {
            var client = new MongoClient(_mongoSettings.ConnectionString);
            var database = client.GetDatabase(_mongoSettings.DatabaseName);

            var pingResult = await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(
                new MongoDB.Bson.BsonDocument("ping", 1));

            var serverStatus = await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(
                new MongoDB.Bson.BsonDocument("serverStatus", 1));

            return new
            {
                Status = "Connected",
                DatabaseName = _mongoSettings.DatabaseName,
                ServerVersion = serverStatus.GetValue("version", "Unknown").ToString(),
                ServerHost = serverStatus.GetValue("host", "Unknown").ToString(),
                ConnectionString = MaskConnectionString(_mongoSettings.ConnectionString),
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get MongoDB connection info");
            return new
            {
                Status = "Error",
                Message = ex.Message,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Checks if MongoDB is accessible
    /// </summary>
    public async Task<bool> IsAccessibleAsync()
    {
        try
        {
            var client = new MongoClient(_mongoSettings.ConnectionString);
            var database = client.GetDatabase(_mongoSettings.DatabaseName);
            
            await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(
                new MongoDB.Bson.BsonDocument("ping", 1));
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void ValidateEnvironmentVariables()
    {
        var mongoPassword = Environment.GetEnvironmentVariable("MONGODB_PASSWORD");
        
        if (string.IsNullOrEmpty(mongoPassword))
        {
            _logger.LogError("❌ MONGODB_PASSWORD environment variable not found");
            _logger.LogError("💡 Please set the MONGODB_PASSWORD environment variable or create a .env file");
            _logger.LogError("   Example: export MONGODB_PASSWORD=\"your_password_here\"");
            _logger.LogError("   Or create a .env file with: MONGODB_PASSWORD=your_password_here");
            throw new InvalidOperationException("MONGODB_PASSWORD environment variable not configured");
        }
        else
        {
            _logger.LogInformation("✅ MONGODB_PASSWORD environment variable found");
        }

        // Check if .env file exists
        var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(envFilePath))
        {
            _logger.LogInformation("✅ .env file found at: {EnvFilePath}", envFilePath);
        }
        else
        {
            _logger.LogInformation("ℹ️  .env file not found - using system environment variables");
        }
    }

    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(_mongoSettings.ConnectionString))
        {
            _logger.LogError("❌ MongoDB ConnectionString not configured");
            throw new InvalidOperationException("MongoDB ConnectionString not configured");
        }

        if (string.IsNullOrEmpty(_mongoSettings.DatabaseName))
        {
            _logger.LogError("❌ MongoDB DatabaseName not configured");
            throw new InvalidOperationException("MongoDB DatabaseName not configured");
        }
    }

    private void LogConnectionSuccess(MongoDB.Bson.BsonDocument serverStatus)
    {
        var serverVersion = serverStatus.GetValue("version", "Unknown").ToString();
        var serverHost = serverStatus.GetValue("host", "Unknown").ToString();

        _logger.LogInformation("✅ MongoDB connection successful!");
        _logger.LogInformation("📊 Database: {DatabaseName}", _mongoSettings.DatabaseName);
        _logger.LogInformation("🖥️  Server: {ServerHost}", serverHost);
        _logger.LogInformation("📦 MongoDB Version: {ServerVersion}", serverVersion);
    }

    private void LogCollectionConfiguration()
    {
        _logger.LogInformation("🔍 Verifying collection configuration...");
        
        var collections = new Dictionary<string, string>
        {
            { "Properties", _mongoSettings.PropertiesCollectionName },
            { "Owners", _mongoSettings.OwnersCollectionName },
            { "PropertyTraces", _mongoSettings.PropertyTracesCollectionName },
            { "Users", _mongoSettings.UsersCollectionName }
        };

        foreach (var collection in collections)
        {
            if (string.IsNullOrEmpty(collection.Value))
            {
                _logger.LogWarning("⚠️  Collection {CollectionType} not configured", collection.Key);
            }
            else
            {
                _logger.LogInformation("📁 Collection {CollectionType}: {CollectionName}", 
                    collection.Key, collection.Value);
            }
        }
    }

    private void HandleMongoException(MongoException mongoEx)
    {
        _logger.LogError(mongoEx, "❌ MongoDB error during startup: {Message}", mongoEx.Message);
        _logger.LogError("💡 Please verify:");
        _logger.LogError("   - MONGODB_PASSWORD environment variable is set");
        _logger.LogError("   - MongoDB Atlas cluster is active");
        _logger.LogError("   - Your IP is whitelisted in MongoDB Atlas");
        _logger.LogError("   - Credentials are correct");
        
        if (_environment.IsDevelopment())
        {
            _logger.LogWarning("⚠️  Continuing in development mode without MongoDB");
            _logger.LogWarning("   Some endpoints may not work correctly");
        }
        else
        {
            throw new InvalidOperationException($"Cannot start application without MongoDB connection: {mongoEx.Message}");
        }
    }

    private void HandleGeneralException(Exception ex)
    {
        _logger.LogError(ex, "❌ Unexpected error verifying MongoDB: {Message}", ex.Message);
        
        if (_environment.IsDevelopment())
        {
            _logger.LogWarning("⚠️  Continuing in development mode...");
        }
        else
        {
            throw new InvalidOperationException($"Cannot start application without MongoDB connection: {ex.Message}");
        }
    }

    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "Not configured";

        var masked = connectionString;
        var passwordStart = masked.IndexOf("://");
        if (passwordStart > 0)
        {
            var passwordEnd = masked.IndexOf("@", passwordStart);
            if (passwordEnd > passwordStart)
            {
                var userPassPart = masked.Substring(passwordStart + 3, passwordEnd - passwordStart - 3);
                var colonIndex = userPassPart.IndexOf(":");
                if (colonIndex > 0)
                {
                    var user = userPassPart.Substring(0, colonIndex);
                    var maskedUserPass = $"{user}:****";
                    masked = masked.Replace(userPassPart, maskedUserPass);
                }
            }
        }

        return masked;
    }
}
