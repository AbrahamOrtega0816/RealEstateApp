using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RealEstateAPI.Features.Authentication.Models;

/// <summary>
/// Represents a user in the MongoDB database
/// </summary>
public class User
{
    /// <summary>
    /// Unique user identifier in MongoDB
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// User email (unique identifier for login)
    /// </summary>
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password using BCrypt
    /// </summary>
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User first name
    /// </summary>
    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User last name
    /// </summary>
    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User role (Admin, User, Manager)
    /// </summary>
    [BsonElement("role")]
    public string Role { get; set; } = "User";

    /// <summary>
    /// User permissions array for fine-grained access control
    /// </summary>
    [BsonElement("permissions")]
    public List<string> Permissions { get; set; } = new();

    /// <summary>
    /// Indicates if the user account is active
    /// </summary>
    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indicates if the user email is verified
    /// </summary>
    [BsonElement("isEmailVerified")]
    public bool IsEmailVerified { get; set; } = false;

    /// <summary>
    /// Last login timestamp
    /// </summary>
    [BsonElement("lastLogin")]
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// Refresh token for JWT renewal
    /// </summary>
    [BsonElement("refreshToken")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token expiration date
    /// </summary>
    [BsonElement("refreshTokenExpiry")]
    public DateTime? RefreshTokenExpiry { get; set; }

    /// <summary>
    /// Failed login attempts counter
    /// </summary>
    [BsonElement("failedLoginAttempts")]
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// Account lockout until timestamp
    /// </summary>
    [BsonElement("lockoutEnd")]
    public DateTime? LockoutEnd { get; set; }

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
    /// Additional metadata stored as flexible document
    /// </summary>
    [BsonElement("metadata")]
    public BsonDocument? Metadata { get; set; }
}
