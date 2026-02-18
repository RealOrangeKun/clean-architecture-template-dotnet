using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Modules.Users.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsersModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_outbox_messages_unprocessed",
                schema: "users",
                table: "outbox_messages",
                columns: new[] { "occurred_on_utc", "processed_on_utc" },
                filter: "processed_on_utc IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_outbox_messages_unprocessed",
                schema: "users",
                table: "outbox_messages");
        }
    }
}
