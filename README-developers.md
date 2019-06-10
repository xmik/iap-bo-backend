# README for developers

## Development
Compile all C# projects (this will also install all the dependencies):
```
./tasks generate_nuget_config

./tasks build
# OR:
dotnet build
```

Run C# unit tests:
```
./tasks utest
# OR:
dotnet test
```

Run integration tests with [Bats](https://github.com/sstephenson/bats):
```
./tasks itest
```

Run the C# server:
```
IAP_BO_GENERATE_TEST_DATA=true
./tasks run
```

### DB
When you change db schema (e.g. add a column to a table), destroy the current db
 docker container, start new (exit from dojo and enter into new), an then:
```
./tasks build
```

then, either recreate db schema:
```
rm -rf Migrations-old
mv src/BranchOfficeBackend/Migrations/ Migrations-old
dotnet ef dbcontext info --startup-project=src/BranchOfficeBackend/ --project=src/BranchOfficeBackend/
dotnet ef migrations add InitialCreate --startup-project=src/BranchOfficeBackend/ --project=src/BranchOfficeBackend/
dotnet ef database update --startup-project=src/BranchOfficeBackend/ --project=src/BranchOfficeBackend/
```

or instead just add a new migration:
```
dotnet ef migrations add TODOChangeName --startup-project=src/BranchOfficeBackend/ --project=src/BranchOfficeBackend/
dotnet ef database update --startup-project=src/BranchOfficeBackend/ --project=src/BranchOfficeBackend/
```

Login into the postgres container and check current state of tables:
```
$ docker exec -ti # ...
postgres-container$ psql -h localhost -U postgres
# list databases
postgres=# \list
postgres=# \connect mydb
# You are now connected to database "mydb" as user "postgres".
# list tables
mydb=# \dt
                  List of relations
 Schema |          Name           | Type  |  Owner   
--------+-------------------------+-------+----------
 public | EmployeeHoursCollection | table | postgres
 public | Employees               | table | postgres
 public | __EFMigrationsHistory   | table | postgres
(3 rows)
mydb=# SELECT * FROM "Employees";
 EmployeeId |      Name       |     Email      |     DateOfBirth     | IsManager
------------+-----------------+----------------+---------------------+-----------
          1 | Jan Kowalski    | jan@gmail.com  | 0001-01-01 00:00:00 | f
          2 | Krzysztof Nowak | krzy@gmail.com | 0001-01-01 00:00:00 | f
          3 | Ala Jeden       | ala1@gmail.com | 0001-01-01 00:00:00 | f
          4 | Ola Dwa         | ola2@gmail.com | 0001-01-01 00:00:00 | f
(4 rows)
```

After running the C# application:
```
./tasks build
./tasks run
```

Warning: the primary keys in PostgreSQL tables (e.g. EmployeeId, EmployeeHoursId)
 must start from 1. If you insert 2 rows with Ids: 0 and 1, this will result in
 in conflict, because the row with Id 0 will have Id 1. Error then will be:
```
Unhandled Exception: Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while updating the entries. See the inner exception for details. ---> Npgsql.PostgresException: 23505: duplicate key value violates unique constraint "PK_EmployeeHoursCollection"
```

### Install Bats on Linux
```
git clone --depth 1 https://github.com/sstephenson/bats.git /opt/bats
git clone --depth 1 https://github.com/ztombol/bats-support.git /opt/bats-support
git clone --depth 1 https://github.com/ztombol/bats-assert.git /opt/bats-assert
/opt/bats/install.sh /usr/local
```

## How the C# solution was set up
```
set -e

# Create a C# .sln file (solution)
dotnet new sln --name BranchOfficeBackend

# Create a C# project (console project)
mkdir -p src/BranchOfficeBackend
( cd src/BranchOfficeBackend; dotnet new console; )

# Reference the console C# project from the .sln file
dotnet sln ./BranchOfficeBackend.sln add src/BranchOfficeBackend/BranchOfficeBackend.csproj

# Create a C# project with tests
mkdir -p tests/BranchOfficeBackend.Tests
( cd tests/BranchOfficeBackend.Tests; dotnet new xunit; )

# Reference the tests C# project from the .sln file
dotnet sln ./BranchOfficeBackend.sln add tests/BranchOfficeBackend.Tests/BranchOfficeBackend.Tests.csproj

# Reference the console C# project from the tests C# project
dotnet add tests/BranchOfficeBackend.Tests/BranchOfficeBackend.Tests.csproj reference src/BranchOfficeBackend/BranchOfficeBackend.csproj
```

## How external dependencies were added
Add external packages references to the C# projects files
```
dotnet add src/BranchOfficeBackend/ package --version=2.1.2 Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/BranchOfficeBackend/ package Microsoft.EntityFrameworkCore.Design
dotnet add src/BranchOfficeBackend/ package --version=4.4.0 Autofac.Extensions.DependencyInjection
dotnet add src/BranchOfficeBackend/ package --version=4.1.0 Carter
dotnet add src/BranchOfficeBackend/ package --version=2.2.0 Microsoft.AspNetCore.Hosting
dotnet add src/BranchOfficeBackend/ package --version=2.2.0 Microsoft.AspNetCore.Server.Kestrel
dotnet add tests/BranchOfficeBackend.Tests/ package RichardSzalay.MockHttp --version 5.0.0
```


## nice docs
* https://www.newtonsoft.com/json/help/html/DeepEquals.htm
* https://mindbyte.nl/http-apis/2018/09/21/Use-Carter-to-create-a-simple-HTTP-API.html
* dotnet-sdk-2.1, [here](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/install) are instructions how to install or use [Dojo](https://github.com/ai-traders/dojo).
* https://docs.microsoft.com/en-us/ef/#pivot=entityfmwk&panel=entityfmwk1 (as a Nuget package)
* https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite - tutorial
* http://www.npgsql.org/efcore/mapping/general.html
* https://www.postgresql.org/docs/current/app-psql.html
* http://www.entityframeworktutorial.net/efcore/entity-framework-core-console-application.aspx
