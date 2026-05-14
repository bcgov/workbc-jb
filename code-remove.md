Yes — safely removable once V2 has baked in prod. Suggested wait: 2–4 weeks of clean daily cron runs + at least one bulk --reimport exercise.

  What can be removed (8 places):

  ┌────────────────────────────────────────────┬────────────────────────────────────────────────────────────────────────────────────────────────────────┐
  │                   Where                    │                                             What to delete                                             │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ src/WorkBC.Importers.Federal/              │ Whole directory (the C# console app)                                                                   │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ src/WorkBC.JobBoard.sln                    │ The WorkBC.Importers.Federal project reference line                                                    │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ src/docker-compose.yml                     │ importers-federal: service block                                                                       │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ src/docker-compose.linux-dev.yml           │ importers-federal: overlay block                                                                       │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ src/dotnet-cli.Dockerfile                  │ Line 35: COPY --from=src_importers-federal /app /app/workbc-importers-federal                          │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ src/docker-compose.linux-dev.yml           │ The dotnet-cli mount line:                                                                             │
  │                                            │ ./WorkBC.Importers.Federal/appsettings.linux.json:/app/workbc-importers-federal/appsettings.json       │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ .github/workflows/build.yml,               │ "Tag/Push the Federal Importer docker image" blocks                                                    │
  │ build-test.yml, build-noc.yml              │                                                                                                        │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ .github/workflows/tag-test.yml,            │ jb-importers-federal pull/tag/push blocks                                                              │
  │ tag-prod.yml, tag.yaml                     │                                                                                                        │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ .github/workflows/deploy-dev.yml,          │ The IMAGE6 env var declaration (already not referenced by kubectl set image)                           │
  │ deploy-test.yaml, deploy-prod.yaml         │                                                                                                        │
  ├────────────────────────────────────────────┼────────────────────────────────────────────────────────────────────────────────────────────────────────┤
  │ ECR                                        │ Optional: delete the jb-importers-federal repo after rollback window closes                            │
  └────────────────────────────────────────────┴────────────────────────────────────────────────────────────────────────────────────────────────────────┘

  What MUST stay (still in active use):
  - src/WorkBC.Indexers.Federal/ — C# indexer that reads ImportedJobsFederal and writes to ES. V2 only replaces the importer (stage 1); indexer is stage 2.
  - src/WorkBC.ElasticSearch.Indexing/Services/XmlParsingServiceFederal.cs + XmlParsingHelpers/FederalXmlLocations.cs — used by the indexer above.
  - src/WorkBC.Tests/Fixtures/FederalXmlJobs/ — XML test fixtures the indexer tests use.

  Rollback nuance: while IMAGE6 and the build/push blocks remain, rollback is one-line (federal-importer=$IMAGE12 → federal-importer=$IMAGE6 in
  deploy-*.yaml). Once you delete those blocks, rollback requires re-adding the build steps + rebuilding from a tagged commit. So the practical pattern is:

  1. Day 0: V2 activated (just done).
  2. Week 2–4: confirm V2 staging counts, ES doc parity, no operator reports.
  3. Then: a single PR with the deletions above. Branch merges to master → re-tag → re-deploy.
