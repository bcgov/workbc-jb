#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM src_jb-dotnet-build AS dotnet-build
WORKDIR /src
COPY ["WorkBC.Indexers.Wanted/", "WorkBC.Indexers.Wanted/"]
RUN dotnet restore "WorkBC.Indexers.Wanted/WorkBC.Indexers.Wanted.csproj"
WORKDIR "/src/WorkBC.Indexers.Wanted"
RUN dotnet publish "WorkBC.Indexers.Wanted.csproj" -c Release -o /app/publish --no-restore --no-dependencies

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
RUN cp /usr/share/zoneinfo/America/Vancouver /etc/localtime && \
    echo "America/Vancouver" > /etc/timezone
COPY --from=dotnet-build /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Indexers.Wanted.dll"]