#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /src

COPY ["WorkBC.Shared/WorkBC.Shared.csproj", "WorkBC.Shared/"]
COPY ["WorkBC.Data/WorkBC.Data.csproj", "WorkBC.Data/"]
COPY ["WorkBC.ElasticSearch.Indexing/WorkBC.ElasticSearch.Indexing.csproj", "WorkBC.ElasticSearch.Indexing/"]
COPY ["WorkBC.ElasticSearch.Models/WorkBC.ElasticSearch.Models.csproj", "WorkBC.ElasticSearch.Models/"]
COPY ["WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj", "WorkBC.ElasticSearch.Search/"]

RUN dotnet restore "WorkBC.ElasticSearch.Models/WorkBC.ElasticSearch.Models.csproj"
RUN dotnet restore "WorkBC.Data/WorkBC.Data.csproj"
RUN dotnet restore "WorkBC.Shared/WorkBC.Shared.csproj"
RUN dotnet restore "WorkBC.ElasticSearch.Indexing/WorkBC.ElasticSearch.Indexing.csproj"
RUN dotnet restore "WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj"

COPY ["WorkBC.ElasticSearch.Models/", "WorkBC.ElasticSearch.Models/"]
COPY ["WorkBC.Data/", "WorkBC.Data/"]
COPY ["WorkBC.Shared/", "WorkBC.Shared/"]
COPY ["WorkBC.ElasticSearch.Indexing/", "WorkBC.ElasticSearch.Indexing/"]
COPY ["WorkBC.ElasticSearch.Search/", "WorkBC.ElasticSearch.Search/"]

RUN dotnet build "WorkBC.ElasticSearch.Models/WorkBC.ElasticSearch.Models.csproj" -c Release
RUN dotnet build "WorkBC.Data/WorkBC.Data.csproj" -c Release
RUN dotnet build "WorkBC.Shared/WorkBC.Shared.csproj" -c Release
RUN dotnet build "WorkBC.ElasticSearch.Indexing/WorkBC.ElasticSearch.Indexing.csproj" -c Release
RUN dotnet build "WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj" -c Release
