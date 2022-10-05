#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Run webpack in a node image
FROM node:14 AS node
WORKDIR /src
COPY ["WorkBC.Admin/package.json", "WorkBC.Admin/"]
COPY ["WorkBC.Admin/package-lock.json", "WorkBC.Admin/"]
COPY ["WorkBC.Admin/webpack.config.js", "WorkBC.Admin/"]
COPY ["WorkBC.Admin/webpack.config.vendor.js", "WorkBC.Admin/"]
COPY ["WorkBC.Admin/wwwroot", "WorkBC.Admin/wwwroot/"]
COPY ["WorkBC.Admin/Styles", "WorkBC.Admin/Styles/"]
RUN rm -rf WorkBC.Admin/wwwroot/dist
WORKDIR /src/WorkBC.Admin
RUN npm install
RUN npm run install
ARG NODE_ENV=production
WORKDIR /src/WorkBC.Admin
RUN ./node_modules/.bin/webpack --progress --mode production

# Build the .NET core app in a dotnet sdk image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk
WORKDIR /src
COPY ["WorkBC.Admin/WorkBC.Admin.csproj", "WorkBC.Admin/"]
COPY ["WorkBC.Shared/WorkBC.Shared.csproj", "WorkBC.Shared/"]
COPY ["WorkBC.Data/WorkBC.Data.csproj", "WorkBC.Data/"]
COPY ["WorkBC.ElasticSearch.Models/WorkBC.ElasticSearch.Models.csproj", "WorkBC.ElasticSearch.Models/"]
RUN dotnet restore "WorkBC.Admin/WorkBC.Admin.csproj"
COPY . .
RUN rm -rf WorkBC.Admin/wwwroot/dist
WORKDIR "/src/WorkBC.Admin"
RUN dotnet publish "WorkBC.Admin.csproj" -c Release -o /app/publish

# Copy both build outputs to the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=sdk /app/publish .
COPY --from=node /src/WorkBC.Admin/wwwroot/dist wwwroot/dist
EXPOSE 8080
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Admin.dll"]