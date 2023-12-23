
### To create a migration

- In your development environment, install the migration tools globally using: 
  `dotnet tool install --global dotnet-ef --version 6.*`
- Change directory to: `.\src\WorkBC.Web`
- List the possible database contexts (required below): `dotnet-ef dbcontext list`
- Create a new migration using: `dotnet ef migrations add <SomeMigrationName> --context JobBoardContext --project ..\WorkBC.Data`

