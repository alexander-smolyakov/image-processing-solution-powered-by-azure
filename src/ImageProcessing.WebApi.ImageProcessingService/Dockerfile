FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ImageProcessing.WebApi.ImageProcessingService/ImageProcessing.WebApi.ImageProcessingService.csproj", "ImageProcessing.WebApi.ImageProcessingService/"]
RUN dotnet restore "ImageProcessing.WebApi.ImageProcessingService/ImageProcessing.WebApi.ImageProcessingService.csproj"
COPY . .
WORKDIR "/src/ImageProcessing.WebApi.ImageProcessingService"
RUN dotnet build "ImageProcessing.WebApi.ImageProcessingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageProcessing.WebApi.ImageProcessingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageProcessing.WebApi.ImageProcessingService.dll"]