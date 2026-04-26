## Backend Dockerfile (CairoTransportation API)
# ---------------------------------------------------
# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# copy csproj and restore as distinct layer
COPY *.csproj ./
RUN dotnet restore
# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish /p:PublishReadyToRun=true

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "CairoTransportation.dll"]