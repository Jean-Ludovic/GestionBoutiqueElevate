# ---------- Étape 1 : Build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie le fichier projet et restaure les dépendances
COPY *.csproj ./
RUN dotnet restore

# Copie tout le reste et publie
COPY . ./
RUN dotnet publish -c Release -o /app/out

# ---------- Étape 2 : Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Configure le port utilisé par Render
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Copie le résultat du build
COPY --from=build /app/out ./

# Expose le port 8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GestionBoutiqueElevate.dll"]
