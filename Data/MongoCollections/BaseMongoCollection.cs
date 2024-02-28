using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Balo.Data.MongoCollections
{
    public class BaseMongoCollection
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }
}
