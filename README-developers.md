# README for developers

## Development
Compile all C# projects (this will also install all the dependencies):
```
./tasks build
# OR:
dotnet build
```

Run C# tests:
```
./tasks test
# OR:
dotnet test
```

Run integration tests with [Bats](https://github.com/sstephenson/bats):
```
./tasks itest
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
```
