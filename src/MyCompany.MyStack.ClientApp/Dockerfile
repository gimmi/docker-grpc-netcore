FROM microsoft/dotnet:2.2-runtime

WORKDIR /app

COPY bin/Release/netcoreapp2.2/publish ./

ENTRYPOINT ["dotnet", "MyCompany.MyStack.ClientApp.dll"]
