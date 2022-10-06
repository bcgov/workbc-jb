#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM src_my-sdk AS sdk
WORKDIR /src
COPY ["WorkBC.Indexers.Wanted/WorkBC.Indexers.Wanted.csproj", "WorkBC.Indexers.Wanted/"]
RUN dotnet restore "WorkBC.Indexers.Wanted/WorkBC.Indexers.Wanted.csproj"
COPY . .
WORKDIR "/src/WorkBC.Indexers.Wanted"
RUN dotnet publish "WorkBC.Indexers.Wanted.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /app
COPY --from=sdk /app/publish .
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Indexers.Wanted.dll"]