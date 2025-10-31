using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment1.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id-1234",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDYVYGiMt0MOuCNJ1ASr6U9gFlAYiVpFnCmXQFpQCXXRTg4Zb7fg7LgAiyc2o70HZA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "regular-user-id-5678",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDYVYGiMt0MOuCNJ1ASr6U9gFlAYiVpFnCmXQFpQCXXRTg4Zb7fg7LgAiyc2o70HZA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id-1234",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELgGE2LKzO8BdnqU9H2a8Lf3KlCJmPUJ8q2C+x1bI7B9Q/9DgEQUdDpqzQeq6V8g6Q==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "regular-user-id-5678",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJPQGE3LKzO8BdnqU9H2a8Lf3KlCJmPUJ8q2C+x1bI7B9Q/9DgEQUdDpqzQeq6V8g7R==");
        }
    }
}
