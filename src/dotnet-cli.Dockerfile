#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0

COPY --from=src_importers-wanted /app /app/workbc-importers-wanted
COPY --from=src_importers-federal /app /app/workbc-importers-federal
COPY --from=src_indexers-wanted /app /app/workbc-indexers-wanted
COPY --from=src_indexers-federal /app /app/workbc-indexers-federal
COPY --from=src_notifications-job-alerts /app /app/workbc-notifications-jobalerts
COPY --from=src_migration-runner /app /app/efmigrationrunner

ENTRYPOINT ["/bin/bash"]