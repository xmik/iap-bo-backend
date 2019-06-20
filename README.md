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
./tasks up
```

Stopping and removing the containers:
```
# press Ctrl+C and then:
./tasks down
```

## API Server endpoints examples

Add employees one at a time:
```
$ curl -i -X POST localhost:8080/api/employees -d '{"email": "123@gmail.com", "name": "Alex Nowy"}' -H 'Content-Type: text/json; charset=utf-8'
$ curl -i -X POST localhost:8080/api/employees -d '{"email": "jan@gmail.com", "name": "Jan Kowalski", "isManager": "true" }' -H 'Content-Type: text/json; charset=utf-8'
```

Get list of all employees:
```
$ curl -i  localhost:8080/api/employees/list
[
    {"name":"Alex Nowy","email":"123@gmail.com","id":1,"isManager":false},
    {"name":"Jan Kowalski","email":"jan@gmail.com","id":2,"isManager":true}
]
```

Get one employee:
```
$ curl -i  localhost:8080/api/employees/1
{"name":"Jan Kowalski","email":"jan@gmail.com","id":1,"isManager":false}
```

Add employee_hours objects one by one (employees with ids: 1 and 2 must have been already created):
```
$ curl -i -X POST localhost:8080/api/employee_hours -d '{"value": "100", "employeeId": "1", "timePeriod": "17.06.2019-23.06.2019" }' -H 'Content-Type: text/json; charset=utf-8'
$ curl -i -X POST localhost:8080/api/employee_hours -d '{"value": "45", "employeeId": "1", "timePeriod": "24.06.2019-30.06.2019" }' -H 'Content-Type: text/json; charset=utf-8'

$ curl -i -X POST localhost:8080/api/employee_hours -d '{"value": "22", "employeeId": "2", "timePeriod": "17.06.2019-23.06.2019" }' -H 'Content-Type: text/json; charset=utf-8'
```



Get one employee_hours object:
```
$ curl -i  localhost:8080/api/employee_hours/1
{"value":100.0,"timePeriod":"17.06.2019-23.06.2019","id":1,"employeeId":1}
```

Delete one employee_hours object:
```
curl -i -X DELETE localhost:8080/api/employee_hours/1
```

Get a collection of employee_hours for one employee with id=1:
```
$ curl -i  localhost:8080/api/employee_hours/list/1
[
    {"value":100.0,"timePeriod":"17.06.2019-23.06.2019","id":1,"employeeId":1},
    {"value":45.0,"timePeriod":"24.06.2019-30.06.2019","id":2,"employeeId":1}
]
```
Get a collection of employee_hours objects for all employees
```
$ curl -i  localhost:8080/api/employee_hours/list_all
[
    {"value":100.0,"timePeriod":"17.06.2019-23.06.2019","id":1,"employeeId":1},
    {"value":45.0,"timePeriod":"24.06.2019-30.06.2019","id":2,"employeeId":1},
    {"value":22.0,"timePeriod":"17.06.2019-23.06.2019","id":3,"employeeId":2}
]
```

Invoke synchronization:
```
$ curl -i -X POST localhost:8080/api/synchronize -d ''
```


The API Server specification is kept in files: `swagger-branch-office.yaml` and `swagger-headquarters.yaml`.
In order to render it as a pretty http website, copy each of those files contents onto: https://editor.swagger.io/


## Load tests
Run load tests interactively with:
```
./tasks load_test
```
and then visit localhost:8089 is your browser.

Or run them notinteractively with:
```
./tasks load_test_notinteractive
```

## Dependencies
* docker
* docker-compose
