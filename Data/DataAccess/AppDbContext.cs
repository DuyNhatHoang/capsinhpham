using Data.MongoCollections;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataAccess
{
    public class AppDbContext
    {
        private readonly IMongoDatabase _db;
        private IMongoClient _mongoClient;

        public AppDbContext(IMongoClient client, string databaseName)
        {
            _db = client.GetDatabase(databaseName);
            _mongoClient = client;
        }

        public IMongoCollection<OrderItem> OrderItem => _db.GetCollection<OrderItem>("orderItem");
        public IMongoCollection<WorkingSession> WorkingSession => _db.GetCollection<WorkingSession>("workingSession");
        public IMongoCollection<Product> Products => _db.GetCollection<Product>("products");
        public IMongoCollection<UserFirebaseToken> UserFirebaseTokens => _db.GetCollection<UserFirebaseToken>("firebaseToken");
        public IClientSessionHandle StartSession()
        {
            return _mongoClient.StartSession();
        }

        public void CreateCollectionsIfNotExists()
        {
            var collectionNames = _db.ListCollectionNames().ToList();
            if (!collectionNames.Any(name => name == "orderItem"))
            {
                _db.CreateCollection("orderItem");
            }
            if (!collectionNames.Any(name => name == "workingSession"))
            {
                _db.CreateCollection("workingSession");
            }
            if (!collectionNames.Any(name => name == "products"))
            {
                _db.CreateCollection("products");
            }
            if (!collectionNames.Any(name => name == "firebaseToken"))
            {
                _db.CreateCollection("firebaseToken");
            }
        }
    }
}
