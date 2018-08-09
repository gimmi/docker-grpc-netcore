FROM microsoft/dotnet:2.1-runtime

WORKDIR /app

COPY bin/Release/netcoreapp2.1/publish ./

ENTRYPOINT ["dotnet", "MyCompany.MyStack.ClientApp.dll"]
