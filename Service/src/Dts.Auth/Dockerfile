# mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim + vsdbg
FROM aspnet-vsdbg:2.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Dts.Auth/Dts.Auth.csproj", "Dts.Auth/"]
COPY ["Dts.Core/Dts.Core.csproj", "Dts.Core/"]
RUN dotnet restore "Dts.Auth/Dts.Auth.csproj"
COPY . .
WORKDIR /src/Dts.Auth
RUN dotnet build "Dts.Auth.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Dts.Auth.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Dts.Auth.dll"]