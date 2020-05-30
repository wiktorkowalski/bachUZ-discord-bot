#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BachUZ.Discord/BachUZ.Discord.csproj", "BachUZ.Discord/"]
WORKDIR "/src/BachUZ.Discord"
RUN dotnet add package Discord.Net -s https://www.myget.org/F/discord-net/api/v3/index.json -v 2.3.0-dev-20200525.6
WORKDIR /src
RUN dotnet restore "BachUZ.Discord/BachUZ.Discord.csproj"
COPY . .
WORKDIR "/src/BachUZ.Discord"
RUN dotnet build "BachUZ.Discord.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BachUZ.Discord.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BachUZ.Discord.dll"]