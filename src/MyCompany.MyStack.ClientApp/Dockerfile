FROM mcr.microsoft.com/dotnet/core/sdk:2.2.203 AS build

COPY ./ ./workspace

RUN dotnet publish workspace/src/MyCompany.MyStack.ClientApp/MyCompany.MyStack.ClientApp.csproj --configuration Release

FROM mcr.microsoft.com/dotnet/core/runtime:2.2.4

WORKDIR /app

COPY --from=build workspace/src/MyCompany.MyStack.ClientApp/bin/Release/netcoreapp2.2/publish/ .

ENTRYPOINT ["dotnet", "MyCompany.MyStack.ClientApp.dll"]
