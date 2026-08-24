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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberName = table.Column<string>(type: "TEXT", nullable: false),
                    memberEmail = table.Column<string>(type: "TEXT", nullable: false),
                    PhotoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checkouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheckOutId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckedOutAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ReturnedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkouts_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CheckoutId = table.Column<int>(type: "INTEGER", nullable: true),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                    table.ForeignKey(
                        name: "FK_Books_Checkouts_CheckoutId",
                        column: x => x.CheckoutId,
                        principalTable: "Checkouts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CheckoutItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CheckoutItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckoutId = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckoutItems_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckoutItems_Checkouts_CheckoutId",
                        column: x => x.CheckoutId,
                        principalTable: "Checkouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_CheckoutId",
                table: "Books",
                column: "CheckoutId");

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
                name: "IX_CheckoutItems_CheckoutId",
                table: "CheckoutItems",
                column: "CheckoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_MemberId",
                table: "Checkouts",
                column: "MemberId");
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
