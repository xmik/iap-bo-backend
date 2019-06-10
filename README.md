# iap-bo-backend

A school project for Internet Application Programming.
Branch office backend, written in C#.

## Usage
Compile code (this will also install all the dependencies):
```
./tasks build
```

Build docker image:
```
./tasks docker_build
```

## Demo - Features presentation

Here we run the server (BO Backend + postgresql db) interactively, so that all the messages are printed onto terminal:
```
IAP_BO_GENERATE_TEST_DATA=true
docker-compose -f ./ops/docker-compose.yml up
```

Stopping and removing the containers:
```
# press Ctrl+C and then:
docker-compose -f ./ops/docker-compose.yml down
```

## API Server endpoints

Endpoints examples:
```
curl -i  localhost:8080/api/employees/list
```

The API Server specification is kept in files: `swagger-branch-office.yaml` and `swagger-headquarters.yaml`.
In order to render it as a pretty http website, copy each of those files contents onto: https://editor.swagger.io/


## Dependencies
* docker
* docker-compose
