using System.IO;
using System.Reflection;

namespace Scraper.Contracts
{
    public static class Contracts
    {
        public static class GQLContracts
        {
            private const string baseDir = "./gql-schemas";
            public static string Scraper => File.ReadAllText(
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                    baseDir,
                    "scraper.gql"));
        }
    }
}