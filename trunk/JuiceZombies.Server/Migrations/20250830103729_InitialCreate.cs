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
                name: "GachaDatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Pity_IdCounter = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GachaDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroDatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ConfigId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    ItemType = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Exp = table.Column<int>(type: "integer", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: true),
                    Quality = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopDatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsBuyADCard = table.Column<bool>(type: "boolean", nullable: false),
                    IsBuyMonthCard = table.Column<bool>(type: "boolean", nullable: false),
                    BuyedMonthCardms = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_GachaDatas_UserId",
                table: "GachaDatas",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroDatas_UserId",
                table: "HeroDatas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopDatas_UserId",
                table: "ShopDatas",
                column: "UserId",
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
                name: "GachaDatas");

            migrationBuilder.DropTable(
                name: "HeroDatas");

            migrationBuilder.DropTable(
                name: "ShopDatas");

            migrationBuilder.DropTable(
                name: "UserResDatas");
        }
    }
}
