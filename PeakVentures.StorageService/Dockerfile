#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PeakVentures.StorageService/PeakVentures.StorageService.csproj", "PeakVentures.StorageService/"]
RUN dotnet restore "PeakVentures.StorageService/PeakVentures.StorageService.csproj"
COPY . .
WORKDIR "/src/PeakVentures.StorageService"
RUN dotnet build "PeakVentures.StorageService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PeakVentures.StorageService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PeakVentures.StorageService.dll"]