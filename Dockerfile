# Gradnja in prevzem (Build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Kopiranje projektnih datotek za namestitev odvisnosti
COPY ["src/AssignmentService.Api/AssignmentService.Api.csproj", "src/AssignmentService.Api/"]
COPY ["src/AssignmentService.Application/AssignmentService.Application.csproj", "src/AssignmentService.Application/"]
COPY ["src/AssignmentService.Domain/AssignmentService.Domain.csproj", "src/AssignmentService.Domain/"]
COPY ["src/AssignmentService.Infrastructure/AssignmentService.Infrastructure.csproj", "src/AssignmentService.Infrastructure/"]

RUN dotnet restore "src/AssignmentService.Api/AssignmentService.Api.csproj"

# Kopiranje preostale izvorne kode
COPY . .
WORKDIR "/src/src/AssignmentService.Api"
RUN dotnet publish "AssignmentService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Končni zagon
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AssignmentService.Api.dll"]