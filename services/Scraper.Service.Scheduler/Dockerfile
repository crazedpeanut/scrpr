FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

COPY ./.ruleset ./
COPY ./Scraper.Service.Scheduler ./Scraper.Service.Scheduler
COPY ./Scraper.Service.Core ./Scraper.Service.Core
COPY ./Scraper.Contracts ./Scraper.Contracts
COPY ./Scraper.Configuration ./Scraper.Configuration
COPY ./Scraper.Mongo ./Scraper.Mongo

RUN dotnet restore ./Scraper.Service.Scheduler/Scraper.Service.Scheduler.csproj
RUN dotnet build --no-restore ./Scraper.Service.Scheduler/Scraper.Service.Scheduler.csproj
RUN dotnet publish -o publish --no-build --no-restore ./Scraper.Service.Scheduler/Scraper.Service.Scheduler.csproj

FROM  mcr.microsoft.com/dotnet/core/aspnet:3.1 as publish

COPY --from=build ./publish/. ./

CMD ["dotnet", "Scraper.Service.Scheduler.dll"]