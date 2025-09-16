using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StargateAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "John Doe" },
                    { 2, "Jane Doe" }
                });

                migrationBuilder.InsertData(
                    table: "AstronautDetail",
                    columns: new[] { "Id", "CareerEndDate", "CareerStartDate", "CurrentDutyTitle", "CurrentRank", "PersonId" },
                    values: new object[] { 1, null, new DateTime(2025, 9, 11, 15, 4, 2, 622, DateTimeKind.Local).AddTicks(9373), "Commander", "1LT", 1 });

                migrationBuilder.InsertData(
                    table: "AstronautDuty",
                    columns: new[] { "Id", "DutyEndDate", "DutyStartDate", "DutyTitle", "PersonId", "Rank" },
                    values: new object[] { 1, null, new DateTime(2025, 9, 11, 15, 4, 2, 622, DateTimeKind.Local).AddTicks(9433), "Commander", 1, "1LT" });
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                migrationBuilder.DeleteData(
                table: "AstronautDetail",
                keyColumn: "Id",
                keyValue: 1);

                migrationBuilder.DeleteData(
                    table: "AstronautDuty",
                    keyColumn: "Id",
                    keyValue: 1);

                migrationBuilder.DeleteData(
                    table: "Person",
                    keyColumn: "Id",
                    keyValue: 2);

                migrationBuilder.DeleteData(
                    table: "Person",
                    keyColumn: "Id",
                    keyValue: 1);
            }
        }
    }
}
