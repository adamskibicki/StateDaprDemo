FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StateDaprDemo/StateDaprDemo.csproj", "StateDaprDemo/"]
RUN dotnet restore "StateDaprDemo/StateDaprDemo.csproj"
COPY . .
WORKDIR "/src/StateDaprDemo"
RUN dotnet build "StateDaprDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StateDaprDemo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StateDaprDemo.dll"]
