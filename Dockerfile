FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY CryptexAPI/CryptexAPI.sln .

COPY CryptexAPI/CryptexAPI/CryptexAPI.csproj CryptexAPI/

RUN dotnet restore "CryptexAPI.sln"

COPY . .

RUN dotnet publish "CryptexAPI/CryptexAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "CryptexAPI.dll"]
