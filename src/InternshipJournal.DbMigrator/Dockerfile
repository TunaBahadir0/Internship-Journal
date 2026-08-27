FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY common.props .
COPY src/InternshipJournal.Domain.Shared/InternshipJournal.Domain.Shared.csproj src/InternshipJournal.Domain.Shared/
COPY src/InternshipJournal.Domain/InternshipJournal.Domain.csproj src/InternshipJournal.Domain/
COPY src/InternshipJournal.Application.Contracts/InternshipJournal.Application.Contracts.csproj src/InternshipJournal.Application.Contracts/
COPY src/InternshipJournal.EntityFrameworkCore/InternshipJournal.EntityFrameworkCore.csproj src/InternshipJournal.EntityFrameworkCore/
COPY src/InternshipJournal.DbMigrator/InternshipJournal.DbMigrator.csproj src/InternshipJournal.DbMigrator/
RUN dotnet restore src/InternshipJournal.DbMigrator/InternshipJournal.DbMigrator.csproj

COPY . .
RUN dotnet publish src/InternshipJournal.DbMigrator/InternshipJournal.DbMigrator.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InternshipJournal.DbMigrator.dll"]
