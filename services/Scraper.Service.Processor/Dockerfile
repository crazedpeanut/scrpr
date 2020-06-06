FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

COPY ./.ruleset ./
COPY ./Scraper.Service.Processor ./Scraper.Service.Processor
COPY ./Scraper.Service.Data ./Scraper.Service.Data
COPY ./Shared ./Shared

RUN dotnet restore ./Scraper.Service.Processor/Scraper.Service.Processor.csproj
RUN dotnet build --no-restore ./Scraper.Service.Processor/Scraper.Service.Processor.csproj
RUN dotnet publish -o publish --no-build --no-restore ./Scraper.Service.Processor/Scraper.Service.Processor.csproj

FROM  mcr.microsoft.com/dotnet/core/aspnet:3.1 as publish

COPY --from=build ./publish/* ./

# FIXME: aspnet image bug 
RUN mkdir runtimes && cd runtimes && ln -s ../unix unix

CMD ["dotnet", "Scraper.Service.Processor.dll"]