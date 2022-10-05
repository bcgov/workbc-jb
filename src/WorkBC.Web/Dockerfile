#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Build the Angular app in a node image
FROM node:14 AS node
WORKDIR /src
COPY ["WorkBC.Web/ClientApp", "WorkBC.Web/ClientApp"]
WORKDIR /src/WorkBC.Web/ClientApp
RUN npm install
RUN npm run buildJbLib
RUN npm run buildJbAccount
RUN npm run buildJbSearch

# Build the .NET core app in a dotnet sdk image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk
WORKDIR /src
COPY ["WorkBC.Web/WorkBC.Web.csproj", "WorkBC.Web/"]
COPY ["WorkBC.Shared/WorkBC.Shared.csproj", "WorkBC.Shared/"]
COPY ["WorkBC.Data/WorkBC.Data.csproj", "WorkBC.Data/"]
COPY ["WorkBC.ElasticSearch.Models/WorkBC.ElasticSearch.Models.csproj", "WorkBC.ElasticSearch.Models/"]
COPY ["WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj", "WorkBC.ElasticSearch.Search/"]
RUN dotnet restore "WorkBC.Web/WorkBC.Web.csproj"
COPY . .
RUN rm -rf WorkBC.Web/wwwroot/dist
WORKDIR "/src/WorkBC.Web"
RUN dotnet publish "WorkBC.Web.csproj" -c Release -o /app/publish

# Copy both build outputs to the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 8081
WORKDIR /app
COPY --from=sdk /app/publish .
COPY --from=node /src/WorkBC.Web/wwwroot/dist wwwroot/dist
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Web.dll"]