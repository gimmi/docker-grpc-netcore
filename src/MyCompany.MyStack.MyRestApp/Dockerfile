FROM microsoft/dotnet:2.1-runtime

WORKDIR /app

COPY bin/Release/netcoreapp2.1/publish ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "MyCompany.MyStack.MyRestApp.dll"]
