using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContosoUniversity.Migrations
{
    /// <inheritdoc />
    public partial class ModifedDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa cột hiện tại
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Department");

            // Tạo lại cột với kiểu dữ liệu rowversion
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Department",
                type: "rowversion",
                nullable: true);
        

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa cột hiện tại
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Department");

            // Tạo lại cột với kiểu dữ liệu trước đó (nếu có)
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Department",
                type: "varbinary(max)",
                nullable: true);
        }

    }
}
