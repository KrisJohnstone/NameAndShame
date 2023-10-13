FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NameAndShame/NameAndShame.csproj", "NameAndShame/"]
RUN dotnet restore "NameAndShame/NameAndShame.csproj"
COPY . .
WORKDIR "/src/NameAndShame"
RUN dotnet build "NameAndShame.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NameAndShame.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NameAndShame.dll"]
