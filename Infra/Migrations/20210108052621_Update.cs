using Microsoft.EntityFrameworkCore.Migrations;

namespace Charlie.OpenIam.Infra.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "city",
                table: "organizations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "organizations",
                type: "text",
                nullable: true);
        }
    }
}
