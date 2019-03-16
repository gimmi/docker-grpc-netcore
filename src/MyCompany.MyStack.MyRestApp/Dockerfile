FROM microsoft/dotnet:2.2-runtime

WORKDIR /app

COPY bin/Release/netcoreapp2.2/publish ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "MyCompany.MyStack.MyRestApp.dll"]
