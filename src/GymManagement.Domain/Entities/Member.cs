using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using BCrypt.Net;

namespace GymManagement.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class Member
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("avatar")]
        public string? Avatar { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("gender")]
        public string? Gender { get; set; } // male, female, other

        [BsonElement("phone")]
        public string? Phone { get; set; }

        [BsonElement("dateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [BsonElement("address")]
        public string? Address { get; set; }

        [BsonElement("role")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Role { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "active"; // active, inactive, pending, banned

        [BsonElement("otp")]
        public string? Otp { get; set; }

        [BsonElement("otpExpires")]
        public DateTime? OtpExpires { get; set; }

        [BsonElement("isVerified")]
        public bool IsVerified { get; set; } = false;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool VerifyPassword(string inputPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, this.Password);
        }


    }
}