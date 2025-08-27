using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JuiceZombies.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDiamondsToUserResDatasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Diamonds",
                table: "UserResDatas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Golds",
                table: "UserResDatas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_ShopDatas_GameUserId",
                table: "ShopDatas",
                column: "GameUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShopDatas_GameUserId",
                table: "ShopDatas");

            migrationBuilder.DropColumn(
                name: "Diamonds",
                table: "UserResDatas");

            migrationBuilder.DropColumn(
                name: "Golds",
                table: "UserResDatas");
        }
    }
}
