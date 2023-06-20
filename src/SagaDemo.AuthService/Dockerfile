FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000
ENTRYPOINT [ "dotnet", "SagaDemo.AuthService.dll" ]