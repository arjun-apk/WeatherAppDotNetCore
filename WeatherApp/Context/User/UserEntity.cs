using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WeatherApp.Context.User
{
    public class UserEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [BsonElement("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("mobileNumber")]
        public long MobileNumber { get; set; }

        [BsonElement("isDeleted")]
        public bool IsDeleted { get; set; } = false;

        [BsonElement("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [BsonElement("createdOn")]
        public DateTime CreatedOn { get; set; }

        [BsonElement("updatedBy")]
        public string? UpdatedBy { get; set; }

        [BsonElement("updatedOn")]
        public DateTime? UpdatedOn { get; set; }
    }
}
