FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Cap2Srt.csproj", "./"]
RUN dotnet restore "Cap2Srt.csproj"
COPY . .
RUN dotnet build "Cap2Srt.csproj" -c Release -o /app/build
RUN dotnet publish "Cap2Srt.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cap2Srt.dll"]
