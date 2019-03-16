### Build App

```
dotnet publish .\src\MyCompany.MyStack.sln --configuration Release
```

### Run locally

```
dotnet .\src\MyCompany.MyStack.ServerApp\bin\Release\netcoreapp2.1\publish\MyCompany.MyStack.ServerApp.dll p1 p2 p3
dotnet .\src\MyCompany.MyStack.ClientApp\bin\Release\netcoreapp2.1\publish\MyCompany.MyStack.ClientApp.dll p1 p2 p3
dotnet .\src\MyCompany.MyStack.MyRestApp\bin\Release\netcoreapp2.1\publish\MyCompany.MyStack.MyRestApp.dll
```

### Build Docker Image

```
docker build --tag serverapp.mystack.mycompany .\src\MyCompany.MyStack.ServerApp
docker build --tag clientapp.mystack.mycompany .\src\MyCompany.MyStack.ClientApp
docker build --tag myrestapp.mystack.mycompany .\src\MyCompany.MyStack.MyRestApp
```

### Run Container in background

```
docker network create mystack.mycompany

docker run -d --name serverapp --publish 50052:50052 --network=mystack.mycompany --detach serverapp.mystack.mycompany par1 par2 "par 3"
docker logs --follow serverapp
docker stop serverapp
docker logs --tail 20 serverapp
docker container rm serverapp

docker run -d --name clientapp --network=mystack.mycompany --detach clientapp.mystack.mycompany par1 par2 "par 3"
docker logs --follow clientapp
docker stop clientapp
docker logs --tail 20 clientapp
docker container rm clientapp

docker network rm mystack.mycompany

docker run -d --name myrestapp --publish 5000:5000 myrestapp.mystack.mycompany
docker stop myrestapp
docker container rm myrestapp
```

### Remove image

```
docker image rm serverapp
docker image rm clientapp
docker image rm myrestapp
```
