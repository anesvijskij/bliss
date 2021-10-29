FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Web/Bliss.WorkflowFacade/Bliss.WorkflowFacade.csproj", "Bliss.WorkflowFacade/"]
RUN dotnet restore "Web/Bliss.WorkflowFacade/Bliss.WorkflowFacade.csproj"
COPY . .
WORKDIR "/src/Bliss.WorkflowFacade"
RUN dotnet build "Bliss.WorkflowFacade.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bliss.WorkflowFacade.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bliss.WorkflowFacade.dll"]
