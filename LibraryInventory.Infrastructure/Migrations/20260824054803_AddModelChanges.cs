using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryInventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "memberEmail",
                table: "Members",
                newName: "MemberEmail");

            migrationBuilder.AddColumn<string>(
                name: "MemberAddress",
                table: "Members",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberAddress",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "MemberEmail",
                table: "Members",
                newName: "memberEmail");
        }
    }
}
