FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
#ENV ASPNETCORE_URLS=http://+:5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/host/Eva.Insurtech.Product.HttpApi.Host
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build --no-restore -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eva.Insurtech.FlowManager.HttpApi.Host.dll"]


