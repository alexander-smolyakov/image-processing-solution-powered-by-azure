#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ImageProcessing.TaskManagerService/ImageProcessing.TaskManagerService.csproj", "ImageProcessing.TaskManagerService/"]
RUN dotnet restore "ImageProcessing.TaskManagerService/ImageProcessing.TaskManagerService.csproj"
COPY . .
WORKDIR "/src/ImageProcessing.TaskManagerService"
RUN dotnet build "ImageProcessing.TaskManagerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageProcessing.TaskManagerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageProcessing.TaskManagerService.dll"]