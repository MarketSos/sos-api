FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "src/Services/POS/StoreOS.POS.API/StoreOS.POS.API.csproj"
RUN dotnet build "src/Services/POS/StoreOS.POS.API/StoreOS.POS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Services/POS/StoreOS.POS.API/StoreOS.POS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreOS.POS.API.dll"]
