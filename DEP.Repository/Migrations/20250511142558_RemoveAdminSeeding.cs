using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdminSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "DepartmentId", "EducationBossId", "LocationId", "Name", "PasswordExpiryDate", "PasswordHash", "PasswordSalt", "RefreshToken", "RefreshTokenExpiryDate", "UserName", "UserRole" },
                values: new object[] { 1, null, null, null, "Administrator", new DateTime(2025, 3, 5, 13, 12, 16, 302, DateTimeKind.Local).AddTicks(5579), new byte[] { 3, 251, 131, 89, 121, 174, 168, 23, 251, 96, 255, 182, 24, 92, 167, 254, 174, 191, 61, 165, 5, 138, 109, 128, 240, 72, 126, 167, 74, 91, 229, 75, 64, 98, 239, 151, 99, 209, 57, 132, 60, 133, 44, 208, 165, 18, 107, 20, 66, 250, 68, 73, 61, 152, 189, 195, 150, 245, 149, 151, 36, 134, 75, 8 }, new byte[] { 157, 71, 38, 190, 135, 222, 48, 196, 157, 31, 235, 185, 85, 213, 185, 97, 141, 151, 98, 249, 42, 109, 36, 185, 9, 130, 232, 206, 117, 64, 134, 46, 11, 6, 172, 137, 144, 47, 221, 87, 159, 117, 228, 151, 0, 60, 179, 30, 134, 72, 107, 198, 142, 57, 252, 107, 216, 178, 237, 6, 36, 176, 102, 26, 27, 224, 107, 65, 21, 201, 233, 254, 127, 179, 143, 201, 158, 166, 137, 105, 109, 174, 232, 243, 89, 68, 195, 105, 21, 18, 23, 71, 252, 215, 34, 87, 254, 22, 63, 71, 83, 144, 78, 250, 120, 117, 150, 247, 118, 103, 56, 221, 28, 197, 171, 137, 8, 77, 200, 100, 113, 70, 0, 104, 19, 59, 2, 52 }, null, new DateTime(2025, 3, 7, 13, 12, 16, 302, DateTimeKind.Local).AddTicks(5663), "admin", 0 });
        }
    }
}
