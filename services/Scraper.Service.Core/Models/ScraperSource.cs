using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Scraper.Mongo;

namespace Scraper.Service.Core.Models
{
    [BsonDiscriminator(WebCollectorProperties.Kind)]
    public class WebCollectorProperties : CollectorProperties
    {
        public const string Kind = "web";
        public Uri Target { get; set; }
        public override string CollectorKind => Kind;
    }

    [BsonKnownTypes(typeof(WebCollectorProperties))]
    [BsonDiscriminator(RootClass = true)]
    public abstract class CollectorProperties
    {
        [BsonElement("_t")]
        public abstract string CollectorKind { get; }
    }

    public class ScraperSource : MongoEntity
    {
        public CollectorProperties Collector { get; set; }
        public string OwnerServiceId { get; set; }
    }
}