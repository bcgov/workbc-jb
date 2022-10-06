#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM src_my-sdk AS sdk
WORKDIR /src
COPY ["WorkBC.Importers.Wanted/WorkBC.Importers.Wanted.csproj", "WorkBC.Importers.Wanted/"]
RUN dotnet restore "WorkBC.Importers.Wanted/WorkBC.Importers.Wanted.csproj"
COPY . .
WORKDIR "/src/WorkBC.Importers.Wanted"
RUN dotnet publish "WorkBC.Importers.Wanted.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=sdk /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Importers.Wanted.dll"]