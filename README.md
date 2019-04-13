# iap-bo-backend

A school project for Internet Application Programming.
Branch office backend, written in C#.

## Usage
Compile code (this will also install all the dependencies):
```
./tasks generate_nuget_config
./tasks build
```

Create/update db schema:
```
dotnet ef dbcontext info --startup-project=src/BranchOfficeBackend/ --project=src/BranchOfficeBackend/
dotnet ef migrations add InitialCreate --startup-project=src/BranchOfficeBackend/ --project=src/BranchOfficeBackend/
dotnet ef database update --startup-project=src/BranchOfficeBackend/ --project=src/BranchOfficeBackend/
```

Run the C# server:
```
./tasks run
```

Endpoints examples:
```
curl -i  localhost:8080/employee/list
```


## Dependencies and docs
* dotnet-sdk-2.1, [here](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/install) are instructions how to install or use [Dojo](https://github.com/ai-traders/dojo).
* https://docs.microsoft.com/en-us/ef/#pivot=entityfmwk&panel=entityfmwk1 (as a Nuget package)
* https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite - tutorial
* http://www.npgsql.org/efcore/mapping/general.html
* https://www.postgresql.org/docs/current/app-psql.html
* http://www.entityframeworktutorial.net/efcore/entity-framework-core-console-application.aspx
