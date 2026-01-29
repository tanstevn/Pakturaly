FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS build-env
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/publish .
EXPOSE 8081
ENTRYPOINT ["dotnet", "Pakturaly.Api.dll"]