using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tutorialAsp.NetCore.Migrations
{
    /// <inheritdoc />
    public partial class addRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                nullable: true);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
