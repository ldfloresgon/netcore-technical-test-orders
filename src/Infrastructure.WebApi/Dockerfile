#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Infrastructure.WebApi/Infrastructure.WebApi.csproj", "Infrastructure.WebApi/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Core.csproj", "Domain/"]
RUN dotnet restore "Infrastructure.WebApi/Infrastructure.WebApi.csproj"
COPY . .
WORKDIR "/src/Infrastructure.WebApi"
RUN dotnet build "Infrastructure.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Infrastructure.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infrastructure.WebApi.dll"]