using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace real_time_collaboration.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitedUserToInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvitedUserId",
                table: "Invitations",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitedUserId",
                table: "Invitations");
        }
    }
}
