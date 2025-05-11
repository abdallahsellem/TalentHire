using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TalentHire.Migrations
{
    /// <inheritdoc />
    public partial class applicants_model_jobapplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Job_Users_EmployerId",
                table: "Job");

            migrationBuilder.DropForeignKey(
                name: "FK_Job_Users_UserId",
                table: "Job");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Job",
                table: "Job");

            migrationBuilder.RenameTable(
                name: "Job",
                newName: "Jobs");

            migrationBuilder.RenameIndex(
                name: "IX_Job_UserId",
                table: "Jobs",
                newName: "IX_Jobs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Job_EmployerId",
                table: "Jobs",
                newName: "IX_Jobs_EmployerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    JobSeekerId = table.Column<int>(type: "integer", nullable: false),
                    ResumeURL = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplications_Users_JobSeekerId",
                        column: x => x.JobSeekerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecommendedApplicants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobId = table.Column<int>(type: "integer", nullable: false),
                    RecommendedUserId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendedApplicants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecommendedApplicants_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecommendedApplicants_Users_RecommendedUserId",
                        column: x => x.RecommendedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobId",
                table: "JobApplications",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobSeekerId",
                table: "JobApplications",
                column: "JobSeekerId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendedApplicants_JobId",
                table: "RecommendedApplicants",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_RecommendedApplicants_RecommendedUserId",
                table: "RecommendedApplicants",
                column: "RecommendedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_EmployerId",
                table: "Jobs",
                column: "EmployerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_UserId",
                table: "Jobs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_EmployerId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_UserId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "RecommendedApplicants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jobs",
                table: "Jobs");

            migrationBuilder.RenameTable(
                name: "Jobs",
                newName: "Job");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_UserId",
                table: "Job",
                newName: "IX_Job_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Jobs_EmployerId",
                table: "Job",
                newName: "IX_Job_EmployerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Job",
                table: "Job",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Users_EmployerId",
                table: "Job",
                column: "EmployerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Job_Users_UserId",
                table: "Job",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
