#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /src
COPY ["WorkBC.Shared/WorkBC.Shared.csproj", "WorkBC.Shared/"]
COPY ["WorkBC.Data/WorkBC.Data.csproj", "WorkBC.Data/"]
COPY ["WorkBC.ElasticSearch.Indexing/WorkBC.ElasticSearch.Indexing.csproj", "WorkBC.ElasticSearch.Indexing/"]
COPY ["WorkBC.ElasticSearch.Models/WorkBC.ElasticSearch.Models.csproj", "WorkBC.ElasticSearch.Models/"]
COPY ["WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj", "WorkBC.ElasticSearch.Search/"]
RUN dotnet restore "WorkBC.ElasticSearch.Indexing/WorkBC.ElasticSearch.Indexing.csproj"
RUN dotnet restore "WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj"