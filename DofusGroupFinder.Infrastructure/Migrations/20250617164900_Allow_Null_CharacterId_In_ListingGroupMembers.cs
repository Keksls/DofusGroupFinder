using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DofusGroupFinder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Allow_Null_CharacterId_In_ListingGroupMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListingGroupMembers_Characters_Id",
                table: "ListingGroupMembers");

            migrationBuilder.DropIndex(
                name: "IX_ListingGroupMembers_Id",
                table: "ListingGroupMembers");

            migrationBuilder.CreateIndex(
                name: "IX_ListingGroupMembers_CharacterId",
                table: "ListingGroupMembers",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingGroupMembers_Characters_CharacterId",
                table: "ListingGroupMembers",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListingGroupMembers_Characters_CharacterId",
                table: "ListingGroupMembers");

            migrationBuilder.DropIndex(
                name: "IX_ListingGroupMembers_CharacterId",
                table: "ListingGroupMembers");

            migrationBuilder.CreateIndex(
                name: "IX_ListingGroupMembers_Id",
                table: "ListingGroupMembers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListingGroupMembers_Characters_Id",
                table: "ListingGroupMembers",
                column: "Id",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
