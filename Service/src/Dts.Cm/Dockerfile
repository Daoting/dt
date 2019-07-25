# mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim + vsdbg
FROM aspnet-vsdbg:3.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Dts.Cm/Dts.Cm.csproj", "Dts.Cm/"]
COPY ["Dts.Core/Dts.Core.csproj", "Dts.Core/"]
RUN dotnet restore "Dts.Cm/Dts.Cm.csproj"
COPY . .
WORKDIR /src/Dts.Cm
RUN dotnet build "Dts.Cm.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Dts.Cm.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Dts.Cm.dll"]