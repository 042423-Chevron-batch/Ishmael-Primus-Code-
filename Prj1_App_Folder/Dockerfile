# #stage 1
# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
# WORKDIR /app
# EXPOSE 5037
# ENV ASPNETCORE_URLS=http://+:5037

# #stage 2
# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# WORKDIR /src
# COPY [" Prj1_Api.csproj", "./"]
# RUN dotnet restore "Prj1_Api.csproj"
# COPY . .
# WORKDIR "/src/."
# RUN dotnet build "Prj1_Api.csproj" -c Release -o /app/build

# #stage 3
# FROM build AS publish
# RUN dotnet publish "Prj1_Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# #stage 4
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Prj1_Api.dll"]



#stage 1
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5037
ENV ASPNETCORE_URLS=http://+:5037


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /app


COPY . . 


RUN dotnet restore ./Prj1Api.sln


RUN dotnet publish "Prj1Api.sln" -c Release -o /app/publish /p:UseAppHost=false

# runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet" ,"Prj1_Api.dll"]