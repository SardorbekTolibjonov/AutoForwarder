FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "AutoForwarder.Api/AutoForwarder.Api.csproj"
COPY . .
WORKDIR "/src/AutoForwarder.Api"
RUN dotnet build "AutoForwarder.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoForwarder.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutoForwarder.Api.dll"]
