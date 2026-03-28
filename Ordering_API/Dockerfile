FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Ordering_API/Ordering_API.csproj", "Ordering_API/"]
RUN dotnet restore "Ordering_API/Ordering_API.csproj"
COPY . .
WORKDIR "/src/Ordering_API"
RUN dotnet build "Ordering_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ordering_API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering_API.dll"]
