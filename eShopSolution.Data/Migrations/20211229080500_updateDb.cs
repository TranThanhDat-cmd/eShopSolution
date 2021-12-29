using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eShopSolution.Data.Migrations
{
    public partial class updateDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "4ca48d90-0829-44d3-a12b-f255a6f0c302");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "d258e3c0-619e-4001-8f3e-b8a7dfcc239e", "AQAAAAEAACcQAAAAEO3Mp1UftA31o5pyHDCsESs0hTBBH+NmSUq/Ff2IC593dTBjM8TcGZKEnM9NtNBLAA==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 12, 29, 15, 5, 0, 71, DateTimeKind.Local).AddTicks(5243));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "f104ccc7-ebf9-4b6d-ba93-d87e11f1b825");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0ec7508d-2b93-420f-9510-8ebe3c854ea4", "AQAAAAEAACcQAAAAEMppyFIfXWG0fPtjIV7QvpAJF4UwZLwtjX8fi2CiQdD4CX5pkNN6AE51lGRTIOByKw==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2021, 12, 27, 11, 30, 23, 851, DateTimeKind.Local).AddTicks(8445));
        }
    }
}
