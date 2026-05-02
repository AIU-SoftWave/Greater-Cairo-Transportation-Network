FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CairoTransportation.csproj", "./"]
RUN dotnet restore "CairoTransportation.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CairoTransportation.csproj" -c Release -o /app/build
RUN dotnet publish "CairoTransportation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CairoTransportation.dll"]