# mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim + vsdbg
FROM aspnet-vsdbg:2.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Dt.Auth/Dt.Auth.csproj", "Dt.Auth/"]
COPY ["Dt.Core/Dt.Core.csproj", "Dt.Core/"]
RUN dotnet restore "Dt.Auth/Dt.Auth.csproj"
COPY . .
WORKDIR /src/Dt.Auth
RUN dotnet build "Dt.Auth.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Dt.Auth.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Dt.Auth.dll"]