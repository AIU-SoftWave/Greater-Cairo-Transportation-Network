## Backend Dockerfile (CairoTransportation API)
# ---------------------------------------------------
# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy csproj and restore as distinct layer for better caching
COPY CairoTransportation.csproj ./
RUN dotnet restore
# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
# Create non-root user for security
RUN useradd -m -s /bin/bash appuser && chown -R appuser /app
USER appuser
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "CairoTransportation.dll"]