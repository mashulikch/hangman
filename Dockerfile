FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY ./src/*.csproj ./
RUN dotnet restore
COPY ./src ./
RUN dotnet publish -c Release -o "app.dll"
ENTRYPOINT ["dotnet", "app.dll"]
