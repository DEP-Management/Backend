using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DEP.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addedCourseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordExpiryDate", "PasswordHash", "PasswordSalt", "RefreshTokenExpiryDate" },
                values: new object[] { new DateTime(2025, 3, 5, 13, 12, 16, 302, DateTimeKind.Local).AddTicks(5579), new byte[] { 3, 251, 131, 89, 121, 174, 168, 23, 251, 96, 255, 182, 24, 92, 167, 254, 174, 191, 61, 165, 5, 138, 109, 128, 240, 72, 126, 167, 74, 91, 229, 75, 64, 98, 239, 151, 99, 209, 57, 132, 60, 133, 44, 208, 165, 18, 107, 20, 66, 250, 68, 73, 61, 152, 189, 195, 150, 245, 149, 151, 36, 134, 75, 8 }, new byte[] { 157, 71, 38, 190, 135, 222, 48, 196, 157, 31, 235, 185, 85, 213, 185, 97, 141, 151, 98, 249, 42, 109, 36, 185, 9, 130, 232, 206, 117, 64, 134, 46, 11, 6, 172, 137, 144, 47, 221, 87, 159, 117, 228, 151, 0, 60, 179, 30, 134, 72, 107, 198, 142, 57, 252, 107, 216, 178, 237, 6, 36, 176, 102, 26, 27, 224, 107, 65, 21, 201, 233, 254, 127, 179, 143, 201, 158, 166, 137, 105, 109, 174, 232, 243, 89, 68, 195, 105, 21, 18, 23, 71, 252, 215, 34, 87, 254, 22, 63, 71, 83, 144, 78, 250, 120, 117, 150, 247, 118, 103, 56, 221, 28, 197, 171, 137, 8, 77, 200, 100, 113, 70, 0, 104, 19, 59, 2, 52 }, new DateTime(2025, 3, 7, 13, 12, 16, 302, DateTimeKind.Local).AddTicks(5663) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PasswordExpiryDate", "PasswordHash", "PasswordSalt", "RefreshTokenExpiryDate" },
                values: new object[] { new DateTime(2025, 2, 26, 7, 59, 24, 296, DateTimeKind.Local).AddTicks(1425), new byte[] { 193, 86, 249, 192, 205, 11, 162, 108, 87, 192, 221, 233, 29, 247, 222, 231, 212, 252, 169, 242, 187, 54, 134, 43, 227, 135, 192, 87, 252, 16, 99, 50, 42, 124, 227, 244, 144, 144, 157, 254, 47, 40, 196, 177, 202, 235, 33, 3, 79, 116, 143, 131, 149, 4, 102, 107, 128, 140, 157, 86, 190, 171, 204, 2 }, new byte[] { 132, 136, 67, 216, 103, 174, 219, 211, 215, 87, 191, 149, 179, 162, 131, 45, 99, 178, 150, 154, 174, 51, 234, 234, 129, 6, 159, 116, 247, 50, 102, 145, 144, 64, 0, 158, 78, 219, 47, 113, 90, 28, 37, 55, 122, 23, 167, 76, 226, 253, 16, 149, 35, 160, 186, 36, 42, 155, 7, 92, 226, 171, 143, 13, 221, 38, 32, 246, 60, 247, 202, 184, 203, 230, 71, 103, 73, 165, 163, 159, 238, 133, 159, 42, 150, 219, 229, 230, 60, 14, 146, 135, 98, 29, 32, 158, 14, 121, 62, 203, 64, 175, 65, 243, 246, 231, 70, 22, 119, 132, 169, 86, 83, 31, 189, 0, 144, 94, 85, 186, 167, 197, 245, 250, 230, 177, 226, 155 }, new DateTime(2025, 2, 28, 7, 59, 24, 296, DateTimeKind.Local).AddTicks(1457) });
        }
    }
}
