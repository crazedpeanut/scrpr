using GraphQL;
using GraphQL.Types;

namespace Scraper.Api.Models.GraphQL
{
    [GraphQLMetadata("Collector", IsTypeOf = typeof(Collector))]
    public class CollectorType : UnionGraphType
    {
        public CollectorType()
        {
            Type<WebCollectorType>();
        }
    }

    [GraphQLMetadata("WebCollector", IsTypeOf = typeof(WebCollector))]
    public class WebCollectorType : ObjectGraphType<WebCollector>
    {
    }
}