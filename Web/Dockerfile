#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
COPY ["Abstraction/Abstraction.csproj", "Abstraction/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Infra/Infra.csproj", "Infra/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "Web/Web.csproj"
COPY . .
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 正式环境中，请替换成正式的 https 证书
COPY Web/lics/openIam.pfx /https/openIam.pfx
COPY Web/lics/openIam.crt /usr/local/share/ca-certificates/openIam.crt
RUN update-ca-certificates
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/openIam.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=111111

ENTRYPOINT ["dotnet", "Charlie.OpenIam.Web.dll"]