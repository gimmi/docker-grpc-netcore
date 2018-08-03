### Build App

```
dotnet build --configuration Release .\src\ServerApp.sln
```

### Build Docker Image

```
docker build -t serverapp .
```

### Run Container

```
docker run serverapp par1 par2 "par 3"
```
