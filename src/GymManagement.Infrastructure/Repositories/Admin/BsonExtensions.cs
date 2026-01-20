using MongoDB.Bson;
namespace GymManagement.Infrastructure.Repositories.Admin
{
    public static class BsonExtensions
    {
        public static int GetIntSafe(this BsonDocument doc, string key)
            => doc.GetValue(key, 0).ToInt32();

        public static decimal GetDecimalSafe(this BsonDocument doc, string key)
            => doc.GetValue(key, 0m).ToDecimal();
    }

}
