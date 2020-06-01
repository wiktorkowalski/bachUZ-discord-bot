#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BachUZ.Website/BachUZ.Website.csproj", "BachUZ.Website/"]
RUN dotnet restore "BachUZ.Website/BachUZ.Website.csproj"
COPY . .
WORKDIR "/src/BachUZ.Website"
RUN dotnet build "BachUZ.Website.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BachUZ.Website.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BachUZ.Website.dll"]