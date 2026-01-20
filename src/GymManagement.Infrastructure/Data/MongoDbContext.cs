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
        public IMongoCollection<Membership> Memberships => _database.GetCollection<Membership>("memberships");
        public IMongoCollection<PackageDetail> PackageDetails => _database.GetCollection<PackageDetail>("packagedetails");
        public IMongoCollection<Package> Packages => _database.GetCollection<Package>("packages");
        public IMongoCollection<Payment> Payments => _database.GetCollection<Payment>("payments");
        public IMongoCollection<Promotion> Promotions => _database.GetCollection<Promotion>("promotions");

        public IMongoCollection<Appointment> Appointments => _database.GetCollection<Appointment>("appointments");
        public IMongoCollection<Trainer> Trainers => _database.GetCollection<Trainer>("trainers");

        public IMongoCollection<WorkoutSchedule> WorkoutSchedules => _database.GetCollection<WorkoutSchedule>("WorkoutSchedules");
        public IMongoCollection<Progress> Progress => _database.GetCollection<Progress>("progress");
        public IMongoCollection<BlogPost> BlogPosts => _database.GetCollection<BlogPost>("blogposts");
        public IMongoCollection<BlogCategory> BlogCategories => _database.GetCollection<BlogCategory>("blogcategories");

    }
}