FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

COPY . .
RUN dotnet restore
RUN dotnet build --no-restore
RUN dotnet publish -o publish --no-build --no-restore

FROM  mcr.microsoft.com/dotnet/core/aspnet:3.1 as publish

COPY --from=build ./publish/* ./

CMD ["dotnet", "Api.dll"]