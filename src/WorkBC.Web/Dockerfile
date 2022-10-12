#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Build the Angular app in a node image
FROM node:14 AS node-build
WORKDIR /src
COPY ["WorkBC.Web/ClientApp", "WorkBC.Web/ClientApp"]
WORKDIR /src/WorkBC.Web/ClientApp
RUN npm install
RUN npm run buildJbLib
RUN npm run buildJbAccount
RUN npm run buildJbSearch

# Build the .NET core app in a dotnet sdk image
FROM src_jb-dotnet-build AS dotnet-build
WORKDIR /src
COPY ["WorkBC.Web/", "WorkBC.Web/"]
RUN rm -rf WorkBC.Web/wwwroot/dist
RUN dotnet restore "WorkBC.Web/WorkBC.Web.csproj"
WORKDIR "/src/WorkBC.Web"
RUN dotnet publish "WorkBC.Web.csproj" -c Release -o /app/publish

# Copy both build outputs to the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 8081
WORKDIR /app
COPY --from=dotnet-build /app/publish .
COPY --from=node-build /src/WorkBC.Web/wwwroot/dist wwwroot/dist
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Web.dll"]
