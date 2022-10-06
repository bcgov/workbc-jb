#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM src_my-sdk AS sdk
WORKDIR /src
COPY ["WorkBC.Notifications.JobAlerts/WorkBC.Notifications.JobAlerts.csproj", "WorkBC.Notifications.JobAlerts/"]
RUN dotnet restore "WorkBC.Notifications.JobAlerts/WorkBC.Notifications.JobAlerts.csproj"
COPY . .
WORKDIR "/src/WorkBC.Notifications.JobAlerts"
RUN dotnet publish "WorkBC.Notifications.JobAlerts.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=sdk /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Notifications.JobAlerts.dll"]