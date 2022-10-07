#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#This is an intermediate base image to speed up other container builds

FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /src

COPY ["WorkBC.ElasticSearch.Models/", "WorkBC.ElasticSearch.Models/"]
COPY ["WorkBC.Data/", "WorkBC.Data/"]
COPY ["WorkBC.Shared/", "WorkBC.Shared/"]
COPY ["WorkBC.ElasticSearch.Indexing/", "WorkBC.ElasticSearch.Indexing/"]
COPY ["WorkBC.ElasticSearch.Search/", "WorkBC.ElasticSearch.Search/"]

# only two project needs to be restored & built because they have references to the other three
RUN dotnet restore "WorkBC.ElasticSearch.Indexing/WorkBC.ElasticSearch.Indexing.csproj"
RUN dotnet restore "WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj"

RUN dotnet build "WorkBC.ElasticSearch.Indexing/WorkBC.ElasticSearch.Indexing.csproj" -c Release
RUN dotnet build "WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj" -c Release
