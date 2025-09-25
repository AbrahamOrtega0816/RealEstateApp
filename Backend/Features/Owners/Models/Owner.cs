using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstateAPI.Features.Owners.Models;

/// <summary>
/// Represents an owner in the MongoDB database
/// </summary>
public class Owner
{
    /// <summary>
    /// Unique owner identifier in MongoDB
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Owner name
    /// </summary>
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Owner address
    /// </summary>
    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Owner photo URL
    /// </summary>
    [BsonElement("photo")]
    public string Photo { get; set; } = string.Empty;

    /// <summary>
    /// Owner birthday
    /// </summary>
    [BsonElement("birthday")]
    public DateTime Birthday { get; set; }

    /// <summary>
    /// Record creation date
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last update date
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates if the owner is active
    /// </summary>
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}
