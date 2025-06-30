using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_BE.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientRecordFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ethnicity",
                table: "PatientRecords",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityNumber",
                table: "PatientRecords",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "PatientRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientCode",
                table: "PatientRecords",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ethnicity",
                table: "PatientRecords");

            migrationBuilder.DropColumn(
                name: "IdentityNumber",
                table: "PatientRecords");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "PatientRecords");

            migrationBuilder.DropColumn(
                name: "PatientCode",
                table: "PatientRecords");
        }
    }
}
