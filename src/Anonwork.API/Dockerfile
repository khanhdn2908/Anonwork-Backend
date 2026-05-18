FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Anonwork.API/Anonwork.API.csproj", "src/Anonwork.API/"]
COPY ["src/Anonwork.Application/Anonwork.Application.csproj", "src/Anonwork.Application/"]
COPY ["src/Anonwork.Domain/Anonwork.Domain.csproj", "src/Anonwork.Domain/"]
COPY ["src/Anonwork.Infrastructure/Anonwork.Infrastructure.csproj", "src/Anonwork.Infrastructure/"]

RUN dotnet restore "./src/Anonwork.API/Anonwork.API.csproj"

COPY . .
WORKDIR "/src/src/Anonwork.API"
RUN dotnet build "./Anonwork.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./Anonwork.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "Anonwork.API.dll"]