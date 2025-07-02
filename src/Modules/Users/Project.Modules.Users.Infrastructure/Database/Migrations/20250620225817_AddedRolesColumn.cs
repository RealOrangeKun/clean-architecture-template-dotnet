using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Modules.Users.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "role",
                schema: "users",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                schema: "users",
                table: "users");
        }
    }
}
