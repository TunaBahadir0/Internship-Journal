using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternshipJournal.Migrations
{
    /// <inheritdoc />
    public partial class Added_InternProfile_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppInternProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MentorUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkplaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    University = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SchoolDepartment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StudentNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InternshipPeriod_StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    InternshipPeriod_EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RequiredWorkDays = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInternProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppInternProfiles_AppWorkplaces_WorkplaceId",
                        column: x => x.WorkplaceId,
                        principalTable: "AppWorkplaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppInternProfiles_UserId",
                table: "AppInternProfiles",
                column: "UserId",
                unique: true,
                filter: "\"Status\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AppInternProfiles_WorkplaceId",
                table: "AppInternProfiles",
                column: "WorkplaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppInternProfiles");
        }
    }
}
