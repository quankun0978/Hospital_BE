using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_BE.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Specialties",
                columns: table => new
                {
                    SpecialtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "ntext", nullable: false),
                    Slug = table.Column<string>(type: "ntext", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialties", x => x.SpecialtyId);
                });

            migrationBuilder.CreateTable(
                name: "DoctorClinicSpecialties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClinicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorClinicSpecialties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorClinicSpecialties_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "ClinicId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorClinicSpecialties_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorClinicSpecialties_Users_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorClinicSpecialties_ClinicId",
                table: "DoctorClinicSpecialties",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorClinicSpecialties_DoctorId_ClinicId_SpecialtyId",
                table: "DoctorClinicSpecialties",
                columns: new[] { "DoctorId", "ClinicId", "SpecialtyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorClinicSpecialties_SpecialtyId",
                table: "DoctorClinicSpecialties",
                column: "SpecialtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorClinicSpecialties");

            migrationBuilder.DropTable(
                name: "Specialties");
        }
    }
}
