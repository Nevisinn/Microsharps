FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

# Запускать из родительской папки всех солюшенов (Докер не может работать с папками выше самого себя)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Сохраняем структуру папок в соответствии с ссылками на проекты в оригинале (чтобы не делать лишних `dotnet remove/add reference`)
WORKDIR /src/Infrastructure 
COPY ["Infrastructure/Infrastructure.API/Infrastructure.API.csproj", "Infrastructure.API/"]
COPY ["Infrastructure/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
WORKDIR /src/service
COPY ["ServiceDiscovery/ServiceDiscovery.API/ServiceDiscovery.API.csproj", "ServiceDiscovery.API/"]
COPY ["ServiceDiscovery/ServiceDiscovery.Logic/ServiceDiscovery.Logic.csproj", "ServiceDiscovery.Logic/"]
COPY ["ServiceDiscovery/ServiceDiscovery.Models/ServiceDiscovery.Models.csproj", "ServiceDiscovery.Models/"]
RUN dotnet restore "ServiceDiscovery.API/ServiceDiscovery.API.csproj"
WORKDIR /src
COPY ./ServiceDiscovery ./service
COPY ./Infrastructure ./Infrastructure
WORKDIR "/src/service/ServiceDiscovery.API"
RUN dotnet build "ServiceDiscovery.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceDiscovery.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceDiscovery.API.dll"]
