using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealEstateAPI.Configuration;
using RealEstateAPI.Common.DTOs;
using RealEstateAPI.Features.Shared.Services;
namespace RealEstateAPI.Features.Shared.Controllers;

/// <summary>
/// Controller for system diagnostics
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Diagnostics")]
public class DiagnosticController : ControllerBase
{
    private readonly MongoDbSettings _mongoSettings;
    private readonly ILogger<DiagnosticController> _logger;
    private readonly IMongoDbStartupService _mongoStartupService;

    public DiagnosticController(
        IOptions<MongoDbSettings> mongoSettings,
        ILogger<DiagnosticController> logger,
        IMongoDbStartupService mongoStartupService)
    {
        _mongoSettings = mongoSettings.Value;
        _logger = logger;
        _mongoStartupService = mongoStartupService;
    }

    /// <summary>
    /// Checks the general API status
    /// </summary>
    /// <returns>API status</returns>
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        try
        {
            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            };

            _logger.LogInformation("Health check completed successfully");
            return Ok(ServiceResult<object>.SuccessResult(healthStatus));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during health check");
            return StatusCode(500, ServiceResult<object>.FailureResult("Internal server error"));
        }
    }

    /// <summary>
    /// Verifies MongoDB connection
    /// </summary>
    /// <returns>MongoDB connection status</returns>
    [HttpGet("mongodb/connection")]
    public async Task<IActionResult> TestMongoDbConnection()
    {
        try
        {
            _logger.LogInformation("Starting MongoDB connection test");

            var connectionInfo = await _mongoStartupService.GetConnectionInfoAsync();
            
            _logger.LogInformation("MongoDB connection test completed successfully");
            return Ok(ServiceResult<object>.SuccessResult(connectionInfo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing MongoDB connection: {Message}", ex.Message);
            return StatusCode(500, ServiceResult<object>.FailureResult($"Connection error: {ex.Message}"));
        }
    }

    /// <summary>
    /// Verifies MongoDB collections status
    /// </summary>
    /// <returns>Collections status</returns>
    [HttpGet("mongodb/collections")]
    public async Task<IActionResult> TestMongoDbCollections()
    {
        try
        {
            _logger.LogInformation("Verifying MongoDB collections");

            var client = new MongoClient(_mongoSettings.ConnectionString);
            var database = client.GetDatabase(_mongoSettings.DatabaseName);

            // Get list of existing collections
            var existingCollections = await (await database.ListCollectionNamesAsync()).ToListAsync();

            // Define expected collections
            var expectedCollections = new Dictionary<string, string>
            {
                { "Properties", _mongoSettings.PropertiesCollectionName },
                { "Owners", _mongoSettings.OwnersCollectionName },
                { "PropertyTraces", _mongoSettings.PropertyTracesCollectionName },
                { "Users", _mongoSettings.UsersCollectionName }
            };

            var collectionsStatus = new List<object>();

            foreach (var expectedCollection in expectedCollections)
            {
                var collectionName = expectedCollection.Value;
                var exists = existingCollections.Contains(collectionName);
                
                long documentCount = 0;
                if (exists)
                {
                    var collection = database.GetCollection<MongoDB.Bson.BsonDocument>(collectionName);
                    documentCount = await collection.CountDocumentsAsync(new MongoDB.Bson.BsonDocument());
                }

                collectionsStatus.Add(new
                {
                    Name = expectedCollection.Key,
                    CollectionName = collectionName,
                    Exists = exists,
                    DocumentCount = documentCount,
                    Status = exists ? "OK" : "Missing"
                });
            }

            var result = new
            {
                DatabaseName = _mongoSettings.DatabaseName,
                TotalExistingCollections = existingCollections.Count,
                ExistingCollections = existingCollections,
                ExpectedCollections = collectionsStatus,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Collections verification completed - {ExistingCount} collections found", existingCollections.Count);
            return Ok(ServiceResult<object>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying collections: {Message}", ex.Message);
            return StatusCode(500, ServiceResult<object>.FailureResult($"Error verifying collections: {ex.Message}"));
        }
    }

    /// <summary>
    /// Complete database system test
    /// </summary>
    /// <returns>Complete test results</returns>
    [HttpGet("mongodb/full-test")]
    public async Task<IActionResult> FullMongoDbTest()
    {
        try
        {
            _logger.LogInformation("Starting complete MongoDB test");

            var client = new MongoClient(_mongoSettings.ConnectionString);
            var database = client.GetDatabase(_mongoSettings.DatabaseName);

            // Test 1: Conexión básica
            var pingResult = await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("ping", 1));

            // Test 2: Información del servidor
            var serverStatus = await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("serverStatus", 1));

            // Test 3: Estadísticas de la base de datos
            var dbStats = await database.RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("dbStats", 1));

            // Test 4: Lista de colecciones
            var collections = await (await database.ListCollectionNamesAsync()).ToListAsync();

            // Test 5: Write/read test (temporary collection)
            var testCollectionName = "test_connection_" + DateTime.UtcNow.Ticks;
            var testCollection = database.GetCollection<MongoDB.Bson.BsonDocument>(testCollectionName);
            
            var testDocument = new MongoDB.Bson.BsonDocument
            {
                { "test", "connection" },
                { "timestamp", DateTime.UtcNow },
                { "random", new Random().Next(1000, 9999) }
            };

            await testCollection.InsertOneAsync(testDocument);
            var retrievedDocument = await testCollection.Find(new MongoDB.Bson.BsonDocument()).FirstOrDefaultAsync();
            await database.DropCollectionAsync(testCollectionName);

            var fullTestResult = new
            {
                ConnectionTest = new { Status = "OK", Message = "Ping successful" },
                ServerInfo = new 
                { 
                    Version = serverStatus.GetValue("version", "Unknown"),
                    Uptime = serverStatus.GetValue("uptime", 0),
                    Host = serverStatus.GetValue("host", "Unknown")
                },
                DatabaseInfo = new
                {
                    Name = _mongoSettings.DatabaseName,
                    Collections = collections.Count,
                    DataSize = dbStats.GetValue("dataSize", 0),
                    StorageSize = dbStats.GetValue("storageSize", 0)
                },
                WriteReadTest = new 
                { 
                    Status = retrievedDocument != null ? "OK" : "FAILED",
                    Message = retrievedDocument != null ? "Write and read successful" : "Write/read error"
                },
                Collections = collections,
                Timestamp = DateTime.UtcNow,
                TestDuration = "< 1 second"
            };

            _logger.LogInformation("Complete MongoDB test successful");
            return Ok(ServiceResult<object>.SuccessResult(fullTestResult));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during complete MongoDB test: {Message}", ex.Message);
            return StatusCode(500, ServiceResult<object>.FailureResult($"Error during complete test: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets configuration information (without sensitive data)
    /// </summary>
    /// <returns>Configuration information</returns>
    [HttpGet("configuration")]
    public IActionResult GetConfiguration()
    {
        try
        {
            var config = new
            {
                MongoDB = new
                {
                    DatabaseName = _mongoSettings.DatabaseName,
                    ConnectionString = MaskConnectionString(_mongoSettings.ConnectionString),
                    Collections = new
                    {
                        Properties = _mongoSettings.PropertiesCollectionName,
                        Owners = _mongoSettings.OwnersCollectionName,
                        PropertyTraces = _mongoSettings.PropertyTracesCollectionName,
                        Users = _mongoSettings.UsersCollectionName
                    }
                },
                Environment = new
                {
                    AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                    MongoPasswordConfigured = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGODB_PASSWORD")),
                    Timestamp = DateTime.UtcNow
                }
            };

            return Ok(ServiceResult<object>.SuccessResult(config));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configuration: {Message}", ex.Message);
            return StatusCode(500, ServiceResult<object>.FailureResult($"Error getting configuration: {ex.Message}"));
        }
    }

    /// <summary>
    /// Masks the connection string to avoid exposing credentials
    /// </summary>
    /// <param name="connectionString">Original connection string</param>
    /// <returns>Masked connection string</returns>
    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "Not configured";

        // Reemplazar password con asteriscos
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
