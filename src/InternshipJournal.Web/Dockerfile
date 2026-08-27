FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY common.props .
COPY src/InternshipJournal.Domain.Shared/InternshipJournal.Domain.Shared.csproj src/InternshipJournal.Domain.Shared/
COPY src/InternshipJournal.Domain/InternshipJournal.Domain.csproj src/InternshipJournal.Domain/
COPY src/InternshipJournal.Application.Contracts/InternshipJournal.Application.Contracts.csproj src/InternshipJournal.Application.Contracts/
COPY src/InternshipJournal.Application/InternshipJournal.Application.csproj src/InternshipJournal.Application/
COPY src/InternshipJournal.EntityFrameworkCore/InternshipJournal.EntityFrameworkCore.csproj src/InternshipJournal.EntityFrameworkCore/
COPY src/InternshipJournal.HttpApi/InternshipJournal.HttpApi.csproj src/InternshipJournal.HttpApi/
COPY src/InternshipJournal.HttpApi.Client/InternshipJournal.HttpApi.Client.csproj src/InternshipJournal.HttpApi.Client/
COPY src/InternshipJournal.Web/InternshipJournal.Web.csproj src/InternshipJournal.Web/
RUN dotnet restore src/InternshipJournal.Web/InternshipJournal.Web.csproj

COPY . .
RUN dotnet publish src/InternshipJournal.Web/InternshipJournal.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "InternshipJournal.Web.dll"]
