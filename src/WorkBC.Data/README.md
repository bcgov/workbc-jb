
### To create a migration

- In your development environment, install the migration tools globally using: 
  `dotnet tool install --global dotnet-ef --version 6.*`
- Change directory to: `.\src`
- List the possible database contexts (required below): `dotnet-ef dbcontext list`
- Create a new migration using: `dotnet ef migrations add <SomeMigrationName> --project WorkBC.Data --startup-project WorkBC.Web --context JobBoardContext`

### To test the migration locally
- Change directory to: `.\src`
- Run the migration using: `dotnet ef database update --project WorkBC.Data --startup-project WorkBC.Web --context JobBoardContext`

