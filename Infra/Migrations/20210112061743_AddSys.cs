using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Charlie.OpenIam.Infra.Migrations
{
    public partial class AddSys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sysinfo",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    isjobnounique = table.Column<bool>(nullable: false),
                    isuserphoneunique = table.Column<bool>(nullable: false),
                    isphonepwdloginenabled = table.Column<bool>(nullable: false),
                    isjobnopwdloginenabled = table.Column<bool>(nullable: false),
                    isregisteruserenabled = table.Column<bool>(nullable: false),
                    createdby = table.Column<string>(nullable: true),
                    createdat = table.Column<DateTime>(nullable: false),
                    lastupdatedby = table.Column<string>(nullable: true),
                    lastupdatedat = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sysinfo", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sysinfo");
        }
    }
}
