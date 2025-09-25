using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstateAPI.Features.Properties.Models;

/// <summary>
/// Represents a real estate property in the MongoDB database
/// </summary>
public class Property
{
    /// <summary>
    /// Unique property identifier in MongoDB
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Owner identifier
    /// </summary>
    [BsonElement("idOwner")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string IdOwner { get; set; } = string.Empty;

    /// <summary>
    /// Property name
    /// </summary>
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Property address
    /// </summary>
    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Property price
    /// </summary>
    [BsonElement("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// Property image URL
    /// </summary>
    [BsonElement("image")]
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Internal property code
    /// </summary>
    [BsonElement("codeInternal")]
    public string CodeInternal { get; set; } = string.Empty;

    /// <summary>
    /// Property year of construction
    /// </summary>
    [BsonElement("year")]
    public int Year { get; set; }

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
    /// Indicates if the property is active
    /// </summary>
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;
}
