### Build App

```
dotnet publish .\src\ServerApp.sln --configuration Release
```

### Run locally

```
dotnet .\src\ServerApp\bin\Release\netcoreapp2.1\publish\ServerApp.dll p1 p2 p3
```

### Build Docker Image

```
docker build -t serverapp .
```

### Run Container in background

```
docker run --name serverapp --detach serverapp par1 par2 "par 3"
docker logs serverapp
docker stop serverapp
docker logs serverapp
docker container rm serverapp
```
