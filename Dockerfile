# prepare nuget
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY *.sln .
COPY ErikPortfolioApi/*.csproj ./ErikPortfolioApi/
RUN dotnet restore

COPY ErikPortfolioApi/. ./ErikPortfolioApi/
WORKDIR /source/ErikPortfolioApi
RUN dotnet public -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ErikPortfolioApi.dll"]