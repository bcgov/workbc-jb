#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM src_jb-dotnet-build AS dotnet-build
WORKDIR /src
COPY ["WorkBC.Indexers.Federal/", "WorkBC.Indexers.Federal/"]
RUN dotnet restore "WorkBC.Indexers.Federal/WorkBC.Indexers.Federal.csproj"
WORKDIR "/src/WorkBC.Indexers.Federal"
RUN dotnet publish "WorkBC.Indexers.Federal.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=dotnet-build /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Indexers.Federal.dll"]