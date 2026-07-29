using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace real_time_collaboration.Migrations
{
    /// <inheritdoc />
    public partial class remove_role_in_inv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Invitations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Invitations",
                type: "text",
                nullable: true);
        }
    }
}
