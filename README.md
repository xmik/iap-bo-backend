# iap-bo-backend

A school project for Internet Application Programming.
Branch office backend, written in C#.

## Usage
Run docker containers:
```
$ dojo
```

Login into the postgres container and check current state of tables:
```
$ docker exec -ti # ...
postgres-container$ psql -h localhost -U postgres
# list databases
postgres=# \list
postgres=# \connect postgres
# list tables
postgres=# \dt
# no tables
```

Then in the default dojo container run:
```
dotnet ef dbcontext info --startup-project=src/BOBackendConsole/ --project=src/BOBackendConsole/
dotnet ef migrations add InitialCreate --startup-project=src/BOBackendConsole/ --project=src/BOBackendConsole/
dotnet ef database update --startup-project=src/BOBackendConsole/ --project=src/BOBackendConsole/
```

Now in the postgres container, there are some tables created but no rows in the Blogs table:
```
postgres=# \dt
                 List of relations
 Schema |         Name          | Type  |  Owner   
--------+-----------------------+-------+----------
 public | Blogs                 | table | postgres
 public | Posts                 | table | postgres
 public | __EFMigrationsHistory | table | postgres
(3 rows)
postgres=# SELECT * FROM "Blogs";
 BlogId | Url
--------+-----
(0 rows)
```

After running the C# application:
```
./tasks build
./tasks run
```

There are data in the Blogs table:
```
postgres=# SELECT * FROM "Blogs";
 BlogId |             Url              
--------+------------------------------
      1 | http://blogs.msdn.com/adonet
(1 row)
```

## Dependencies and docs
* dotnet-sdk-2.1, [here](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial/install) are instructions how to install or use [Dojo](https://github.com/ai-traders/dojo).
* https://docs.microsoft.com/en-us/ef/#pivot=entityfmwk&panel=entityfmwk1 (as a Nuget package)
* https://docs.microsoft.com/en-us/ef/core/get-started/netcore/new-db-sqlite - tutorial
* http://www.npgsql.org/efcore/mapping/general.html
* https://www.postgresql.org/docs/current/app-psql.html
