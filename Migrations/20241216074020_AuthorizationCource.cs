using ContosoUniversity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContosoUniversity.Migrations
{
    /// <inheritdoc />
    public partial class AuthorizationCource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột OwnerID
            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);

            // Thêm cột Status
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Course",
                type: "int",
                nullable: false,
                defaultValue: 0);
            
            var newUser = new ContosoUser
            {
                UserName = "newuser@example.com",
                Email = "newuser@example.com",
                EmailConfirmed = true
            };
            // Băm mật khẩu trước khi lưu
            var hasher = new PasswordHasher<ContosoUser>();
            string hashedPassword = hasher.HashPassword(newUser, "P@ssword123!");

            // Cập nhật password hash vào người dùng
            newUser.PasswordHash = hashedPassword;
            // Thêm bản ghi vào AspNetUsers và cập nhật OwnerID trong Course
            migrationBuilder.Sql($@"
                -- Thêm người dùng vào bảng AspNetUsers
                INSERT INTO AspNetUsers 
                (Id, UserName, NormalizedUserName, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
                VALUES ('new-user-id', '{newUser.UserName}', 'NEWUSER@EXAMPLE.COM', 1, '{newUser.PasswordHash}', NEWID(), NEWID(), 0, 0, 1, 0);

                -- Gán OwnerID trong bảng Course
                UPDATE Course
                SET OwnerID = 'new-user-id'
                WHERE OwnerID IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa cột OwnerID
            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Course");

            // Xóa cột Status
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Course");

            // Xóa người dùng đã thêm
            migrationBuilder.Sql(@"
                DELETE FROM AspNetUsers
                WHERE Id = 'new-user-id';
            ");
        }
    }
}
