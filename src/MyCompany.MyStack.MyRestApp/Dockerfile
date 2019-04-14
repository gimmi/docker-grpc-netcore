FROM mcr.microsoft.com/dotnet/core/sdk:2.2.203 AS build

COPY ./ ./workspace

RUN dotnet publish workspace/src/MyCompany.MyStack.MyRestApp/MyCompany.MyStack.MyRestApp.csproj --configuration Release

FROM mcr.microsoft.com/dotnet/core/runtime:2.2.4

WORKDIR /app

COPY --from=build workspace/src/MyCompany.MyStack.MyRestApp/bin/Release/netcoreapp2.2/publish/ .

EXPOSE 5000

ENTRYPOINT ["dotnet", "MyCompany.MyStack.MyRestApp.dll"]
