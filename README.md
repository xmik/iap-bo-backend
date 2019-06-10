# iap-bo-backend

A school project for Internet Application Programming.
Branch office backend, written in C#.

## Usage
### Prerequisities
Compile code (this will also install all the dependencies):
```
./tasks build
```

Build docker image:
```
./tasks docker_build
```

### Run

Here we run the server (BO Backend + postgresql db) interactively, so that all the messages are printed onto terminal:
```
docker-compose -f ./ops/docker-compose.yml up
```

Stopping and removing the containers:
```
# press Ctrl+C and then:
docker-compose -f ./ops/docker-compose.yml down
```

## API Server endpoints examples

Get list of all employees:
```
$ curl -i  localhost:8080/api/employees/list
[{"name":"Jan Kowalski","email":"jan@gmail.com","id":1,"isManager":false},{"name":"Krzysztof Nowak","email":"krzy@gmail.com","id":2,"isManager":false},{"name":"Ala Jeden","email":"ala1@gmail.com","id":3,"isManager":false},{"name":"Ola Dwa","email":"ola2@gmail.com","id":4,"isManager":false}]
```

Get one employee:
```
$ curl -i  localhost:8080/api/employees/1
{"name":"Jan Kowalski","email":"jan@gmail.com","id":1,"isManager":false}
```

Get list of employee_hours for one employee with id=1:
```
$ curl -i  localhost:8080/api/employee_hours/list/1
[{"value":15.0,"timePeriod":"20.1.2019-26.01.2019","id":1,"employeeId":1},{"value":10.0,"timePeriod":"27.01.2019-02.02.2019","id":2,"employeeId":1},{"value":12.0,"timePeriod":"03.02.2019-09.02.2019","id":3,"employeeId":1}]
```

Add one employee:
```
$ curl -i -X POST localhost:8080/api/employees -d '{"email": "123@gmail.com", "name": "Alex Nowy"}' -H 'Content-Type: text/json; charset=utf-8'
```

Invoke synchronization:
```
$ curl -i -X POST localhost:8080/api/synchronize -d ''
```


The API Server specification is kept in files: `swagger-branch-office.yaml` and `swagger-headquarters.yaml`.
In order to render it as a pretty http website, copy each of those files contents onto: https://editor.swagger.io/


## Dependencies
* docker
* docker-compose
