using Microsoft.EntityFrameworkCore.Migrations;

namespace Charlie.OpenIam.Infra.Migrations
{
    public partial class AddMoreUserInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cover",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "github",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "motto",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sinaweibo",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "twitter",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "cover",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "github",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "motto",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "note",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "sinaweibo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "twitter",
                table: "AspNetUsers");
        }
    }
}
