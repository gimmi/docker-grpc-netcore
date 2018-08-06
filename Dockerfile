FROM microsoft/dotnet:2.1-runtime

WORKDIR /app

COPY src/MyCompany.MyStack.ServerApp/bin/Release/netcoreapp2.1/publish ./

ENTRYPOINT ["dotnet", "MyCompany.MyStack.ServerApp.dll"]
