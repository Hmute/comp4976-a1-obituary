using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Assignment1.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Obituaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfDeath = table.Column<string>(type: "TEXT", nullable: false),
                    Biography = table.Column<string>(type: "TEXT", nullable: false),
                    PrimaryPhotoBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obituaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Obituaries_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "admin-user-id-1234", 0, "c8554266-b7ad-4345-b57a-6842bec5b3ab", "aa@aa.aa", true, false, null, "AA@AA.AA", "AA@AA.AA", "AQAAAAIAAYagAAAAELgGE2LKzO8BdnqU9H2a8Lf3KlCJmPUJ8q2C+x1bI7B9Q/9DgEQUdDpqzQeq6V8g6Q==", null, false, "7f434309-a4d9-48e9-9ebb-8803db794577", false, "aa@aa.aa" },
                    { "regular-user-id-5678", 0, "d7b9a1c3-5468-4e72-8d95-8a8a7b5c2b9d", "uu@uu.uu", true, false, null, "UU@UU.UU", "UU@UU.UU", "AQAAAAIAAYagAAAAEJPQGE3LKzO8BdnqU9H2a8Lf3KlCJmPUJ8q2C+x1bI7B9Q/9DgEQUdDpqzQeq6V8g7R==", null, false, "3dba6d67-4142-43e4-b4af-4b2e35f36e7f", false, "uu@uu.uu" }
                });

            migrationBuilder.InsertData(
                table: "Obituaries",
                columns: new[] { "Id", "Biography", "CreatedAtUtc", "CreatedByUserId", "DateOfBirth", "DateOfDeath", "FullName", "PrimaryPhotoBase64" },
                values: new object[,]
                {
                    { 1, "John was a loving father and grandfather who dedicated his life to teaching. He touched the lives of countless students over his 40-year career as a high school mathematics teacher.", new DateTime(2023, 10, 2, 10, 0, 0, 0, DateTimeKind.Utc), "admin-user-id-1234", "1950-05-15", "2023-10-01", "John Doe", null },
                    { 2, "Mary was a passionate nurse who spent 35 years caring for others at the local hospital. She was known for her kindness and dedication to her patients and colleagues.", new DateTime(2024, 3, 16, 14, 30, 0, 0, DateTimeKind.Utc), "regular-user-id-5678", "1965-08-22", "2024-03-15", "Mary Johnson", null },
                    { 3, "Robert was a veteran who served his country with honor for 20 years. After his military service, he became a successful businessman and devoted family man.", new DateTime(2024, 1, 21, 9, 15, 0, 0, DateTimeKind.Utc), "admin-user-id-1234", "1945-12-03", "2024-01-20", "Robert Smith", null },
                    { 4, "Elizabeth was an artist whose beautiful paintings brought joy to many. She taught art classes at the community center and mentored young artists throughout her life.", new DateTime(2024, 5, 13, 16, 45, 0, 0, DateTimeKind.Utc), "regular-user-id-5678", "1958-11-08", "2024-05-12", "Elizabeth Williams", null },
                    { 5, "Michael was a dedicated firefighter who risked his life to save others for over 25 years. He was a hero to his community and a loving husband and father.", new DateTime(2024, 8, 31, 11, 20, 0, 0, DateTimeKind.Utc), "admin-user-id-1234", "1960-02-14", "2024-08-30", "Michael Brown", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Obituaries_CreatedAtUtc",
                table: "Obituaries",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Obituaries_CreatedByUserId",
                table: "Obituaries",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Obituaries_FullName",
                table: "Obituaries",
                column: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Obituaries");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id-1234");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "regular-user-id-5678");
        }
    }
}
