using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tutorialAsp.NetCore.Migrations
{
    /// <inheritdoc />
    public partial class TGID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        { 

            migrationBuilder.AddColumn<string>(
                name: "Tgid",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Tgid",
                table: "Users");
        }
    }
}
