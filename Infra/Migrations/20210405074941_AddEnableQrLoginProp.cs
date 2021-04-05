using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Charlie.OpenIam.Infra.Migrations
{
    public partial class AddEnableQrLoginProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "enabledqrexternallogins",
                table: "sysinfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "enabledqrexternallogins",
                table: "sysinfo");
        }
    }
}
