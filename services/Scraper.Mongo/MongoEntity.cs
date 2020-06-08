using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Scraper.Mongo
{
    public abstract class MongoEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        protected MongoEntity(string id)
        {
            this.Id = id;
        }

        protected MongoEntity()
        { }
    }
}
