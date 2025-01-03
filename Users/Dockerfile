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
COPY ["Users/Users.API/Users.API.csproj", "Users.API/"]
COPY ["Users/Users.Logic/Users.Logic.csproj", "Users.Logic/"]
COPY ["Users/Users.DAL/Users.DAL.csproj", "Users.DAL/"]
COPY ["Users/Users.Models/Users.Models.csproj", "Users.Models/"]
RUN dotnet restore "Users.API/Users.API.csproj"
WORKDIR /src
COPY ./Users ./service
COPY ./Infrastructure ./Infrastructure
WORKDIR "/src/service/Users.API"
RUN dotnet build "Users.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Users.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.API.dll"]
