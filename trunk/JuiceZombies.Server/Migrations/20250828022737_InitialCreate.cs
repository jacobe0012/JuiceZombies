using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JuiceZombies.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShopDatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GameUserId = table.Column<long>(type: "bigint", nullable: false),
                    isBuyADCard = table.Column<bool>(type: "boolean", nullable: false),
                    isBuyMonthCard = table.Column<bool>(type: "boolean", nullable: false),
                    buyedMonthCardms = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserResDatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    Golds = table.Column<long>(type: "bigint", nullable: false),
                    Diamonds = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResDatas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopDatas_GameUserId",
                table: "ShopDatas",
                column: "GameUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserResDatas_UserName",
                table: "UserResDatas",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopDatas");

            migrationBuilder.DropTable(
                name: "UserResDatas");
        }
    }
}
