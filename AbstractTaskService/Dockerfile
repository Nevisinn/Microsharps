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
COPY ["AbstractTaskService/AbstractTaskService.API/AbstractTaskService.API.csproj", "AbstractTaskService.API/"]
COPY ["AbstractTaskService/AbstractTaskService.Logic/AbstractTaskService.Logic.csproj", "AbstractTaskService.Logic/"]
COPY ["AbstractTaskService/AbstractTaskService.Models/AbstractTaskService.Models.csproj", "AbstractTaskService.Models/"]
RUN dotnet restore "AbstractTaskService.API/AbstractTaskService.API.csproj"
WORKDIR /src
COPY ./AbstractTaskService ./service
COPY ./Infrastructure ./Infrastructure
WORKDIR "/src/service/AbstractTaskService.API"
RUN dotnet build "AbstractTaskService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AbstractTaskService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AbstractTaskService.API.dll"]
