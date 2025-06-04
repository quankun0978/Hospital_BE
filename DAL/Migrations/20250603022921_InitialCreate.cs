using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_BE.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allcodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValueEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValueVi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allcodes", x => x.Id);
                    table.UniqueConstraint("AK_Allcodes_CodeKey", x => x.CodeKey);
                });

            migrationBuilder.CreateTable(
                name: "Clinics",
                columns: table => new
                {
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "ntext", nullable: false),
                    Description = table.Column<string>(type: "ntext", nullable: false),
                    Slug = table.Column<string>(type: "ntext", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LogoImg = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsHospital = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinics", x => x.ClinicId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Allcodes_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Allcodes",
                        principalColumn: "CodeKey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoctorInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PositionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Note = table.Column<string>(type: "ntext", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Count = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorInfos_Allcodes_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Allcodes",
                        principalColumn: "CodeKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorInfos_Allcodes_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Allcodes",
                        principalColumn: "CodeKey",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorInfos_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "ClinicId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorInfos_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Markdowns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContentHTML = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentMarkdown = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markdowns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Markdowns_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "ClinicId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Markdowns_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientRecords",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "DATE", nullable: true),
                    Gender = table.Column<string>(type: "nchar(1)", maxLength: 1, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HealthInsuranceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientRecords", x => x.PatientId);
                    table.ForeignKey(
                        name: "FK_PatientRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allcodes_CodeType_CodeKey",
                table: "Allcodes",
                columns: new[] { "CodeType", "CodeKey" });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInfos_ClinicId",
                table: "DoctorInfos",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInfos_DoctorId",
                table: "DoctorInfos",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInfos_PositionId",
                table: "DoctorInfos",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInfos_PriceId",
                table: "DoctorInfos",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_Markdowns_ClinicId",
                table: "Markdowns",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Markdowns_DoctorId",
                table: "Markdowns",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRecords_UserId",
                table: "PatientRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorInfos");

            migrationBuilder.DropTable(
                name: "Markdowns");

            migrationBuilder.DropTable(
                name: "PatientRecords");

            migrationBuilder.DropTable(
                name: "Clinics");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Allcodes");
        }
    }
}
