FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "SagaDemo.OrderService.dll" ]

# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 80

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src

# COPY ["SagaDemo.OrderService.csproj", "src/SagaDemo.OrderService/"]

# RUN dotnet restore "src/SagaDemo.OrderService/SagaDemo.OrderService.csproj"
# COPY . .
# # WORKDIR "/src/SagaDemo.OrderService"
# RUN dotnet build "src/SagaDemo.OrderService/SagaDemo.OrderService.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "src/SagaDemo.OrderService/SagaDemo.OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "SagaDemo.OrderService.dll"]