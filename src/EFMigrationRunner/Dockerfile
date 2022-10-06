#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM src_my-sdk AS sdk
WORKDIR /src
COPY ["EFMigrationRunner/EFMigrationRunner.csproj", "EFMigrationRunner/"]
RUN dotnet restore "EFMigrationRunner/EFMigrationRunner.csproj"
COPY . .
WORKDIR "/src/EFMigrationRunner"
RUN dotnet publish "EFMigrationRunner.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=sdk /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "EFMigrationRunner.dll"]