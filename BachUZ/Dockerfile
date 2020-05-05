#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BachUZ/BachUZ.csproj", "BachUZ/"]
COPY ["BachUZ.Database/BachUZ.Database.csproj", "BachUZ.Database/"]
RUN dotnet restore "BachUZ/BachUZ.csproj"
COPY . .
WORKDIR "/src/BachUZ"
RUN dotnet build "BachUZ.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BachUZ.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BachUZ.dll"]