using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternshipJournal.Migrations
{
    /// <inheritdoc />
    public partial class Added_DailyLog_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDailyLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InternProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogDate = table.Column<DateTime>(type: "date", nullable: false),
                    Summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TotalMinutes = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_AppDailyLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDailyLogs_AppInternProfiles_InternProfileId",
                        column: x => x.InternProfileId,
                        principalTable: "AppInternProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppDailyLogItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    WorkType = table.Column<int>(type: "integer", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    DailyLogId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDailyLogItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDailyLogItems_AppDailyLogs_DailyLogId",
                        column: x => x.DailyLogId,
                        principalTable: "AppDailyLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppDailyLogSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    LearningLevel = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DailyLogId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDailyLogSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDailyLogSkills_AppDailyLogs_DailyLogId",
                        column: x => x.DailyLogId,
                        principalTable: "AppDailyLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppDailyLogSkills_AppSkills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "AppSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppProblemSolvingEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProblemDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AttemptedSolutions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RootCause = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FinalSolution = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UsedArtificialIntelligence = table.Column<bool>(type: "boolean", nullable: false),
                    AiToolName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AiPromptSummary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AiSuggestion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AiSuggestionAccepted = table.Column<bool>(type: "boolean", nullable: true),
                    AiRejectionReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DailyLogId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProblemSolvingEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppProblemSolvingEntries_AppDailyLogs_DailyLogId",
                        column: x => x.DailyLogId,
                        principalTable: "AppDailyLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDailyLogItems_DailyLogId",
                table: "AppDailyLogItems",
                column: "DailyLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDailyLogs_InternProfileId_LogDate",
                table: "AppDailyLogs",
                columns: new[] { "InternProfileId", "LogDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppDailyLogs_Status",
                table: "AppDailyLogs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppDailyLogSkills_DailyLogId_SkillId",
                table: "AppDailyLogSkills",
                columns: new[] { "DailyLogId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppDailyLogSkills_SkillId",
                table: "AppDailyLogSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProblemSolvingEntries_DailyLogId",
                table: "AppProblemSolvingEntries",
                column: "DailyLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDailyLogItems");

            migrationBuilder.DropTable(
                name: "AppDailyLogSkills");

            migrationBuilder.DropTable(
                name: "AppProblemSolvingEntries");

            migrationBuilder.DropTable(
                name: "AppDailyLogs");
        }
    }
}
