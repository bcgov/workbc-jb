#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM src_jb-dotnet-build AS dotnet-build
WORKDIR /src
COPY ["WorkBC.Notifications.JobAlerts/", "WorkBC.Notifications.JobAlerts/"]
RUN dotnet restore "WorkBC.Notifications.JobAlerts/WorkBC.Notifications.JobAlerts.csproj"
WORKDIR "/src/WorkBC.Notifications.JobAlerts"
RUN dotnet publish "WorkBC.Notifications.JobAlerts.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=dotnet-build /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Notifications.JobAlerts.dll"]