FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY real-time-collaboration.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

RUN mkdir -p wwwroot/avatars wwwroot/message-images

EXPOSE 5271

ENV ASPNETCORE_URLS=http://+:5271

ENTRYPOINT ["dotnet", "real-time-collaboration.dll"]
