FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

COPY ./.ruleset ./
COPY ./Scraper.Api ./Scraper.Api
COPY ./Shared ./Shared

RUN dotnet restore ./Scraper.Api/Scraper.Api.csproj
RUN dotnet build --no-restore ./Scraper.Api/Scraper.Api.csproj
RUN dotnet publish -o publish --no-build --no-restore ./Scraper.Api/Scraper.Api.csproj

FROM  mcr.microsoft.com/dotnet/core/aspnet:3.1 as publish

COPY --from=build ./publish/* ./

CMD ["dotnet", "Scraper.Api.dll"]