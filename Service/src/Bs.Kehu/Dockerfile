# mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim + vsdbg
FROM aspnet-vsdbg:3.0 AS base
WORKDIR /app
EXPOSE 443
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Bs.Kehu/Bs.Kehu.csproj", "Bs.Kehu/"]
COPY ["Dt.Core/Dt.Core.csproj", "Dt.Core/"]
RUN dotnet restore "Bs.Kehu/Bs.Kehu.csproj"
COPY . .
WORKDIR /src/Bs.Kehu
RUN dotnet build "Bs.Kehu.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Bs.Kehu.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

# ÈÝÆ÷ÄÚ¼àÌý443
ENV ASPNETCORE_URLS https://*:443
ENTRYPOINT ["dotnet", "Bs.Kehu.dll"]