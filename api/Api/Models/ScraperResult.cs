using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Models
{
    public class ScraperResult : MongoEntity
    {
        public ScraperResult(List<Entity> entities)
        {
            Entities = entities;
        }

        public List<Entity> Entities { get; set; }
    }

    public class Entity
    {
        public Entity(string name, string raw, string source)
        {
            Name = name;
            Raw = raw;
            Source = source;
        }
        public string Name { get; set; }
        public string Raw { get; set; }
        public string Source { get; }
    }
}