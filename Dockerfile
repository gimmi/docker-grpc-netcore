FROM microsoft/dotnet:2.1-runtime

WORKDIR /app

COPY src/ServerApp/bin/Release/netcoreapp2.1 ./

ENTRYPOINT ["dotnet", "ServerApp.dll"]
