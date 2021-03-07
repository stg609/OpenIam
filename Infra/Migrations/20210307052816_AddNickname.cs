using Microsoft.EntityFrameworkCore.Migrations;

namespace Charlie.OpenIam.Infra.Migrations
{
    public partial class AddNickname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "nickname",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nickname",
                table: "AspNetUsers");
        }
    }
}
