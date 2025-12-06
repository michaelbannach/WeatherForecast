
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app


ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src


COPY ["WeatherForecast.Web/WeatherForecast.Web.csproj", "WeatherForecast.Web/"]
COPY ["WeatherForecast.Application/WeatherForecast.Application.csproj", "WeatherForecast.Application/"]
COPY ["WeatherForecast.Infrastructure/WeatherForecast.Infrastructure.csproj", "WeatherForecast.Infrastructure/"]
COPY ["WeatherForecast.Domain/WeatherForecast.Domain.csproj", "WeatherForecast.Domain/"]


RUN dotnet restore "WeatherForecast.Web/WeatherForecast.Web.csproj"

COPY . .


WORKDIR "/src/WeatherForecast.Web"
RUN dotnet publish "WeatherForecast.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app


COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "WeatherForecast.Web.dll"]
