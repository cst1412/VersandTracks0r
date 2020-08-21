 
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY Versandtracks0rBlazor.csproj .
RUN dotnet restore "Versandtracks0rBlazor.csproj"
COPY . .
RUN dotnet build "Versandtracks0rBlazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Versandtracks0rBlazor.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot/ .
COPY nginx.conf /etc/nginx/nginx.conf