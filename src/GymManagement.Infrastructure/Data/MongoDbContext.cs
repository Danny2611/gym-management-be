using GymManagement.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace GymManagement.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration["Database:Mongo:ConnectionString"];
            var databaseName = configuration["Database:Mongo:DatabaseName"];

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Member> Members => _database.GetCollection<Member>("members");
        public IMongoCollection<Role> Roles => _database.GetCollection<Role>("roles");
        // Thêm các collection khác khi cần
        // public IMongoCollection<Package> Packages => _database.GetCollection<Package>("packages");
        // public IMongoCollection<Trainer> Trainers => _database.GetCollection<Trainer>("trainers");
    }
}