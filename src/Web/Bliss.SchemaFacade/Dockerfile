#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Web/Bliss.SchemaFacade/Bliss.SchemaFacade.csproj", "Web/Bliss.SchemaFacade/"]
RUN dotnet restore "Web/Bliss.SchemaFacade/Bliss.SchemaFacade.csproj"
COPY . .
WORKDIR "/src/Web/Bliss.SchemaFacade"
RUN dotnet build "Bliss.SchemaFacade.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bliss.SchemaFacade.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bliss.SchemaFacade.dll"]
