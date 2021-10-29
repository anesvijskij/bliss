FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Web/Bliss.DataFacade/Bliss.DataFacade.csproj", "Bliss.DataFacade/"]
RUN dotnet restore "Web/Bliss.DataFacade/Bliss.DataFacade.csproj"
COPY . .
WORKDIR "/src/Bliss.DataFacade"
RUN dotnet build "Bliss.DataFacade.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bliss.DataFacade.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bliss.DataFacade.dll"]
