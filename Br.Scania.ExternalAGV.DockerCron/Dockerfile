FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Br.Scania.ExternalAGV.DockerCron/Br.Scania.ExternalAGV.DockerCron.csproj Br.Scania.ExternalAGV.DockerCron/
RUN dotnet restore Br.Scania.ExternalAGV.DockerCron/Br.Scania.ExternalAGV.DockerCron.csproj
COPY . .
WORKDIR /src/Br.Scania.ExternalAGV.DockerCron
RUN dotnet build Br.Scania.ExternalAGV.DockerCron.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Br.Scania.ExternalAGV.DockerCron.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Br.Scania.ExternalAGV.DockerCron.dll"]
