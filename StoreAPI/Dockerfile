FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ServiceToService/ServiceToService.csproj", "ServiceToService/"]
RUN dotnet restore "ServiceToService/ServiceToService.csproj"
COPY . .
WORKDIR "/src/ServiceToService"
RUN dotnet build "ServiceToService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceToService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceToService.dll"]
