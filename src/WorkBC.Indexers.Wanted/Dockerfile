#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk
WORKDIR /src
COPY ["WorkBC.Indexers.Wanted/WorkBC.Indexers.Wanted.csproj", "WorkBC.Indexers.Wanted/"]
COPY ["WorkBC.Shared/WorkBC.Shared.csproj", "WorkBC.Shared/"]
COPY ["WorkBC.Data/WorkBC.Data.csproj", "WorkBC.Data/"]
COPY ["WorkBC.ElasticSearch.Models/WorkBC.ElasticSearch.Models.csproj", "WorkBC.ElasticSearch.Models/"]
COPY ["WorkBC.ElasticSearch.Indexing/WorkBC.ElasticSearch.Indexing.csproj", "WorkBC.ElasticSearch.Indexing/"]
RUN dotnet restore "WorkBC.Indexers.Wanted/WorkBC.Indexers.Wanted.csproj"
COPY . .
WORKDIR "/src/WorkBC.Indexers.Wanted"
RUN dotnet publish "WorkBC.Indexers.Wanted.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=sdk /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Indexers.Wanted.dll"]