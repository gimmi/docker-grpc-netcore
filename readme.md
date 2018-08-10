### Build App

```
.\protos\generate.ps1
dotnet publish .\src\MyCompany.MyStack.sln --configuration Release
```

### Run locally

```
dotnet .\src\MyCompany.MyStack.ServerApp\bin\Release\netcoreapp2.1\publish\MyCompany.MyStack.ServerApp.dll p1 p2 p3
dotnet .\src\MyCompany.MyStack.ClientApp\bin\Release\netcoreapp2.1\publish\MyCompany.MyStack.ClientApp.dll p1 p2 p3
```

### Build Docker Image

```
docker build --tag serverapp.mystack.mycompany .\src\MyCompany.MyStack.ServerApp
docker build --tag clientapp.mystack.mycompany .\src\MyCompany.MyStack.ClientApp
```

### Run Container in background

```
docker network create mystack.mycompany

docker run --name serverapp.mystack.mycompany --publish 50052:50052 --network=mystack.mycompany --detach serverapp.mystack.mycompany par1 par2 "par 3"
docker logs --follow serverapp.mystack.mycompany
docker stop serverapp.mystack.mycompany
docker logs --tail 20 serverapp.mystack.mycompany
docker container rm serverapp.mystack.mycompany

docker run --name clientapp.mystack.mycompany  --network=mystack.mycompany --detach clientapp.mystack.mycompany par1 par2 "par 3"
docker logs --follow clientapp.mystack.mycompany
docker stop clientapp.mystack.mycompany
docker logs --tail 20 clientapp.mystack.mycompany
docker container rm clientapp.mystack.mycompany

docker network rm mystack.mycompany
```

### Remove image

```
docker image rm serverapp.mystack.mycompany
docker image rm clientapp.mystack.mycompany
```
