FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["ExampleOrderService.csproj", "./"]

RUN dotnet restore "ExampleOrderService.csproj"

COPY . .

RUN dotnet build "ExampleOrderService.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "ExampleOrderService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "ExampleOrderService.dll"]