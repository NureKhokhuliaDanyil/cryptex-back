using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CryptexAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedFuethersDealEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FuethersDeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TypeOfDeal = table.Column<int>(type: "integer", nullable: false),
                    Leverage = table.Column<int>(type: "integer", nullable: false),
                    TypeOfMargin = table.Column<int>(type: "integer", nullable: false),
                    TakeProfit = table.Column<double>(type: "double precision", nullable: false),
                    StopLoss = table.Column<double>(type: "double precision", nullable: false),
                    EnterPrice = table.Column<double>(type: "double precision", nullable: false),
                    MarginValue = table.Column<double>(type: "double precision", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    CoinId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuethersDeals", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuethersDeals");
        }
    }
}
