# mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim + vsdbg
FROM aspnet-vsdbg:3.0 AS base
WORKDIR /app
EXPOSE 443
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Dt.Fsm/Dt.Fsm.csproj", "Dt.Fsm/"]
COPY ["Dt.Core/Dt.Core.csproj", "Dt.Core/"]
RUN dotnet restore "Dt.Fsm/Dt.Fsm.csproj"
COPY . .
WORKDIR /src/Dt.Fsm
RUN dotnet build "Dt.Fsm.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Dt.Fsm.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# ÈÝÆ÷ÄÚ¼àÌý443
ENV ASPNETCORE_URLS https://*:443
ENTRYPOINT ["dotnet", "Dt.Fsm.dll"]