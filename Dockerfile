FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5135

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ContosoUniversity.csproj",""]
RUN dotnet restore "ContosoUniversity.csproj"
COPY . .
RUN dotnet build "ContosoUniversity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContosoUniversity.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=publish /app/publish .
COPY ["mycert.crt",""]
COPY ["mycert.key",""]
ENTRYPOINT ["dotnet", "ContosoUniversity.dll"]