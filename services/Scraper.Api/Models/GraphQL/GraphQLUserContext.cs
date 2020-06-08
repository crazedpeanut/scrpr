using System.Security.Claims;
using System.Collections.Generic;

namespace Scraper.Api.Models.GraphQL
{
    public class GraphQLUserContext : Dictionary<string, object>
    {
        public ClaimsPrincipal User { get; set; }
    }
}