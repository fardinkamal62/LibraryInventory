using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryInventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "Checkouts",
                columns: table => new
                {
                    CheckOutId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheckedOutAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ReturnedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkouts", x => x.CheckOutId);
                    table.ForeignKey(
                        name: "FK_Checkouts_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Author = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    IsbnNumber = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    PageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PageMaterial = table.Column<string>(type: "TEXT", nullable: false),
                    PhotoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CheckOutId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                    table.ForeignKey(
                        name: "FK_Books_Checkouts_CheckOutId",
                        column: x => x.CheckOutId,
                        principalTable: "Checkouts",
                        principalColumn: "CheckOutId");
                });

            migrationBuilder.CreateTable(
                name: "CheckoutItems",
                columns: table => new
                {
                    CheckoutItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheckoutId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutItems", x => x.CheckoutItemId);
                    table.ForeignKey(
                        name: "FK_CheckoutItems_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckoutItems_Checkouts_CheckoutId",
                        column: x => x.CheckoutId,
                        principalTable: "Checkouts",
                        principalColumn: "CheckOutId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_CheckOutId",
                table: "Books",
                column: "CheckOutId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_IsbnNumber",
                table: "Books",
                column: "IsbnNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutItems_BookId",
                table: "CheckoutItems",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutItems_CheckoutId_BookId",
                table: "CheckoutItems",
                columns: new[] { "CheckoutId", "BookId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_MemberId",
                table: "Checkouts",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_Email",
                table: "Members",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckoutItems");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Checkouts");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
