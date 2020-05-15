# mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim + vsdbg
FROM aspnet-vsdbg:3.0 AS base
WORKDIR /app
EXPOSE 443
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Dt.Ws/Dt.Ws.csproj", "Dt.Ws/"]
COPY ["Dt.Core/Dt.Core.csproj", "Dt.Core/"]
RUN dotnet restore "Dt.Ws/Dt.Ws.csproj"
COPY . .
WORKDIR /src/Dt.Ws
RUN dotnet build "Dt.Ws.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Dt.Ws.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# ÈÝÆ÷ÄÚ¼àÌý443
ENV ASPNETCORE_URLS https://*:443
ENTRYPOINT ["dotnet", "Dt.Ws.dll"]