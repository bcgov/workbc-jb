#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM src_jb-dotnet-build AS dotnet-build
WORKDIR /src
COPY ["WorkBC.Importers.Wanted/", "WorkBC.Importers.Wanted/"]
RUN dotnet restore "WorkBC.Importers.Wanted/WorkBC.Importers.Wanted.csproj"
WORKDIR "/src/WorkBC.Importers.Wanted"
RUN dotnet publish "WorkBC.Importers.Wanted.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=dotnet-build /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Importers.Wanted.dll", "--bulk"]