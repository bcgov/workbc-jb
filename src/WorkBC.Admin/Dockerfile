#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Run webpack in a node image
FROM node:14 AS node-build
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
FROM src_jb-dotnet-build AS dotnet-build
WORKDIR /src
COPY ["WorkBC.Admin/", "WorkBC.Admin/"]
RUN dotnet restore "WorkBC.Admin/WorkBC.Admin.csproj"
RUN rm -rf WorkBC.Admin/wwwroot/dist
WORKDIR "/src/WorkBC.Admin"
RUN dotnet publish "WorkBC.Admin.csproj" -c Release -o /app/publish

# Copy both build outputs to the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=dotnet-build /app/publish .
COPY --from=node-build /src/WorkBC.Admin/wwwroot/dist wwwroot/dist
EXPOSE 8080
ENV Serilog__FileLoggingEnabled "false"
ENTRYPOINT ["dotnet", "WorkBC.Admin.dll"]