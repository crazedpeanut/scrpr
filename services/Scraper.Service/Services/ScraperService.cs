using System.Linq;
using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Scraper.Mongo;
using Scraper.Service.Core.Models;
using Scraper.Service.Core.Services;

namespace Scraper.Service.Services
{
    public class ScraperService : Scraper.ScraperService.ScraperServiceBase
    {
        private readonly IRepository<Core.Models.ScraperSource> scraperSources;
        private readonly ScraperJobPublisher jobPublisher;

        public ScraperService(IRepository<Core.Models.ScraperSource> scraperSources, ScraperJobPublisher jobPublisher)
        {
            this.jobPublisher = jobPublisher;
            this.scraperSources = scraperSources;
        }

        public override async Task<CreateSourceResponse> CreateSource(CreateSourceRequest request, Grpc.Core.ServerCallContext context)
        {
            var source = new Core.Models.ScraperSource()
            {
                Collector = MapCollectorProperties(request.Collector)
            };

            await scraperSources.Create(source, context.CancellationToken);

            return new CreateSourceResponse
            {
                Id = source.Id
            };
        }

        public override async Task<Empty> DeleteSource(DeleteSourceRequest request, Grpc.Core.ServerCallContext context)
        {
            var success = await scraperSources.Delete(request.Id, context.CancellationToken);

            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Source does not exist"));
            }

            return new Empty();
        }

        public override async Task<GetSourceResponse> GetSource(GetSourceRequest request, Grpc.Core.ServerCallContext context)
        {
            var source = await scraperSources.Get(request.Id, context.CancellationToken);

            if (source == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Source does not exist"));
            }

            return new GetSourceResponse
            {
                Result = new ScraperSource
                {
                    Collector = MapCollectorProperties(source.Collector),
                    Id = source.Id
                }
            };
        }

        public override async Task<Empty> UpdateSource(UpdateSourceRequest request, Grpc.Core.ServerCallContext context)
        {
            var source = new Core.Models.ScraperSource()
            {
                Collector = MapCollectorProperties(request.Collector),
                Id = request.Id
            };

            await scraperSources.Update(source, context.CancellationToken);

            return new Empty();
        }

        public override async Task<ListSourcesResponse> ListSources(ListSourcesRequest request, Grpc.Core.ServerCallContext context)
        {
            var sources = await scraperSources.Query(_ => true, request.Skip, request.Take, context.CancellationToken);

            var response = new ListSourcesResponse();
            response.Results.AddRange(sources.Select(_ => new ScraperSource
            {
                Id = _.Id,
                Collector = MapCollectorProperties(_.Collector)
            }));
            return response;
        }

        private static CollectorProperties MapCollectorProperties(Any properties)
        {
            if (properties.Is(WebCollector.Descriptor) && properties.TryUnpack<WebCollector>(out var collector))
            {
                return new WebCollectorProperties
                {
                    Target = new Uri(collector.Target)
                };
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Unknown collector"));
            }
        }

        private static Any MapCollectorProperties(CollectorProperties properties)
        {
            if (properties.CollectorKind == WebCollectorProperties.Kind && properties is WebCollectorProperties value)
            {
                return Any.Pack(new WebCollector
                {
                    Target = value.Target.ToString()
                });
            }

            throw new RpcException(new Status(StatusCode.InvalidArgument, "Unknown collector"));
        }

        public override async Task<StartResponse> Start(StartRequest request, ServerCallContext context)
        {
            var job = new Core.Models.ScraperJob()
            {
                Collector = MapCollectorProperties(request.Collector),
                // FIXME: Sorry excuse for authentication
                RequesterId = context.GetHttpContext().Request.Headers["clientId"].FirstOrDefault()
            };

            return new StartResponse
            {
                Id = await jobPublisher.Publish(job)
            };
        }
    }
}
