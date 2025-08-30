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
                name: "GachaPityCounterDatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Pity_IdCounter = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GachaPityCounterDatas", x => x.Id);
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
                    UserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserResDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemDatas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ConfigId = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    UserResDataId = table.Column<long>(type: "bigint", nullable: true),
                    Exp = table.Column<int>(type: "integer", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: true),
                    Quality = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDatas_UserResDatas_UserResDataId",
                        column: x => x.UserResDataId,
                        principalTable: "UserResDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GachaPityCounterDatas_UserId",
                table: "GachaPityCounterDatas",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemDatas_UserId",
                table: "ItemDatas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDatas_UserResDataId",
                table: "ItemDatas",
                column: "UserResDataId");

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
                name: "GachaPityCounterDatas");

            migrationBuilder.DropTable(
                name: "ItemDatas");

            migrationBuilder.DropTable(
                name: "ShopDatas");

            migrationBuilder.DropTable(
                name: "UserResDatas");
        }
    }
}
