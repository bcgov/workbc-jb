#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk
WORKDIR /src
COPY ["WorkBC.Notifications.JobAlerts/WorkBC.Notifications.JobAlerts.csproj", "WorkBC.Notifications.JobAlerts/"]
COPY ["WorkBC.Shared/WorkBC.Shared.csproj", "WorkBC.Shared/"]
COPY ["WorkBC.Data/WorkBC.Data.csproj", "WorkBC.Data/"]
COPY ["WorkBC.ElasticSearch.Models/WorkBC.ElasticSearch.Models.csproj", "WorkBC.ElasticSearch.Models/"]
COPY ["WorkBC.ElasticSearch.Search/WorkBC.ElasticSearch.Search.csproj", "WorkBC.ElasticSearch.Search/"]
RUN dotnet restore "WorkBC.Notifications.JobAlerts/WorkBC.Notifications.JobAlerts.csproj"
COPY . .
WORKDIR "/src/WorkBC.Notifications.JobAlerts"
RUN dotnet publish "WorkBC.Notifications.JobAlerts.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=sdk /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Notifications.JobAlerts.dll"]