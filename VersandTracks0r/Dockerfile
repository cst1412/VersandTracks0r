 
FROM  mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["VersandTracks0r.csproj", "."]
RUN dotnet restore "VersandTracks0r.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "VersandTracks0r.csproj" -c Release -o /app

FROM build AS publish
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --version 3.1.6
RUN dotnet ef database update
RUN dotnet publish "VersandTracks0r.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "VersandTracks0r.dll","--urls","http://0.0.0.0:80"]