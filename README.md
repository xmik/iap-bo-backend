# iap-bo-backend

A school project for Internet Application Programming.
Branch office backend, written in C#.

## Usage
Compile code (this will also install all the dependencies):
```
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

## API Server endpoints

Endpoints examples:
```
curl -i  localhost:8080/api/employees/list
```

The API Server specification is kept in files: `swagger-branch-office.yaml` and `swagger-headquarters.yaml`.
In order to render it as a pretty http website, copy each of those files output onto: https://editor.swagger.io/


## Dependencies and docs
* dotnet-sdk-2.1, [here](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/install) are instructions how to install or use [Dojo](https://github.com/ai-traders/dojo).
* https://docs.microsoft.com/en-us/ef/#pivot=entityfmwk&panel=entityfmwk1 (as a Nuget package)
* https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite - tutorial
* http://www.npgsql.org/efcore/mapping/general.html
* https://www.postgresql.org/docs/current/app-psql.html
* http://www.entityframeworktutorial.net/efcore/entity-framework-core-console-application.aspx
