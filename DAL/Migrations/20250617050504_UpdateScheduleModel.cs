using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_BE.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScheduleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Allcodes_timeType",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Users_doctorId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "timeType",
                table: "Schedules",
                newName: "TimeType");

            migrationBuilder.RenameColumn(
                name: "doctorId",
                table: "Schedules",
                newName: "DoctorId");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Schedules",
                newName: "Date");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_timeType",
                table: "Schedules",
                newName: "IX_Schedules_TimeType");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_doctorId_date_timeType",
                table: "Schedules",
                newName: "IX_Schedules_DoctorId_Date_TimeType");

            migrationBuilder.AlterColumn<string>(
                name: "TimeType",
                table: "Schedules",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Allcodes_TimeType",
                table: "Schedules",
                column: "TimeType",
                principalTable: "Allcodes",
                principalColumn: "CodeKey",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Users_DoctorId",
                table: "Schedules",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Allcodes_TimeType",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Users_DoctorId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "TimeType",
                table: "Schedules",
                newName: "timeType");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Schedules",
                newName: "doctorId");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Schedules",
                newName: "date");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_TimeType",
                table: "Schedules",
                newName: "IX_Schedules_timeType");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_DoctorId_Date_TimeType",
                table: "Schedules",
                newName: "IX_Schedules_doctorId_date_timeType");

            migrationBuilder.AlterColumn<string>(
                name: "timeType",
                table: "Schedules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Allcodes_timeType",
                table: "Schedules",
                column: "timeType",
                principalTable: "Allcodes",
                principalColumn: "CodeKey",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Users_doctorId",
                table: "Schedules",
                column: "doctorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
