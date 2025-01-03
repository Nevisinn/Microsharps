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
COPY ["Infrastructure/Infrastructure.Client/Infrastructure.Client.csproj", "Infrastructure.Client/"]
WORKDIR /src/ServiceDiscovery
COPY ["ServiceDiscovery/ServiceDiscovery.Models/ServiceDiscovery.Models.csproj", "ServiceDiscovery.Models/"]
WORKDIR /src/AbstractTaskService
COPY ["AbstractTaskService/AbstractTaskService.Client/AbstractTaskService.Client.csproj", "AbstractTaskService.Client/"]
COPY ["AbstractTaskService/AbstractTaskService.Models/AbstractTaskService.Models.csproj", "AbstractTaskService.Models/"]
WORKDIR /src/Users
COPY ["Users/Users.Client/Users.Client.csproj", "Users.Client/"]
COPY ["Users/Users.Models/Users.Models.csproj", "Users.Models/"]
WORKDIR /src/service
COPY ["ApiGateway/ApiGateway/ApiGateway.csproj", "ApiGateway/"]
COPY ["ApiGateway/ApiGateway.Logic/ApiGateway.Logic.csproj", "ApiGateway.Logic/"]
COPY ["ApiGateway/ApiGateway.Models/ApiGateway.Models.csproj", "ApiGateway.Models/"]
RUN dotnet restore "ApiGateway/ApiGateway.csproj"
WORKDIR /src
COPY ./Infrastructure ./Infrastructure
COPY ./ServiceDiscovery ./ServiceDiscovery
COPY ./AbstractTaskService ./AbstractTaskService
COPY ./Users ./Users
COPY ./ApiGateway ./service
WORKDIR "/src/service/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
