using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstateAPI.Features.PropertyTraces.Models;

/// <summary>
/// Represents a property trace/transaction in the MongoDB database
/// </summary>
public class PropertyTrace
{
    /// <summary>
    /// Unique property trace identifier in MongoDB
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Date of the sale/transaction
    /// </summary>
    [BsonElement("dateSale")]
    public DateTime DateSale { get; set; }

    /// <summary>
    /// Transaction name or description
    /// </summary>
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Transaction value
    /// </summary>
    [BsonElement("value")]
    public decimal Value { get; set; }

    /// <summary>
    /// Tax amount
    /// </summary>
    [BsonElement("tax")]
    public decimal Tax { get; set; }

    /// <summary>
    /// Property identifier
    /// </summary>
    [BsonElement("idProperty")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string IdProperty { get; set; } = string.Empty;

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
}
