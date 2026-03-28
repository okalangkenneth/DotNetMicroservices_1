FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Dicount.API/Dicount.API.csproj", "Dicount.API/"]
RUN dotnet restore "Dicount.API/Dicount.API.csproj"
COPY . .
WORKDIR "/src/Dicount.API"
RUN dotnet build "Dicount.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dicount.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dicount.API.dll"]
