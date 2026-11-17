# Build the .NET 10 application.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Rebyulusyon.csproj", "./"]
RUN dotnet restore "Rebyulusyon.csproj"

COPY . .
RUN dotnet publish "Rebyulusyon.csproj" --configuration Release --output /app/publish /p:UseAppHost=false

# Run the published application.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Rebyulusyon.dll"]