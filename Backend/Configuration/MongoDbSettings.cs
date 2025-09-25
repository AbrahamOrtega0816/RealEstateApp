namespace RealEstateAPI.Configuration;

/// <summary>
/// Configuration for MongoDB connection
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// MongoDB connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database name
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>
    /// Properties collection name
    /// </summary>
    public string PropertiesCollectionName { get; set; } = string.Empty;

    /// <summary>
    /// Owners collection name
    /// </summary>
    public string OwnersCollectionName { get; set; } = string.Empty;


    /// <summary>
    /// Property traces collection name
    /// </summary>
    public string PropertyTracesCollectionName { get; set; } = string.Empty;

    /// <summary>
    /// Users collection name
    /// </summary>
    public string UsersCollectionName { get; set; } = string.Empty;
}
