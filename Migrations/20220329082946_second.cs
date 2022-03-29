using Microsoft.EntityFrameworkCore.Migrations;

namespace APL_Technical_Test.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AzureUrl",
                table: "imageInformation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AzureUrl",
                table: "imageInformation");
        }
    }
}
