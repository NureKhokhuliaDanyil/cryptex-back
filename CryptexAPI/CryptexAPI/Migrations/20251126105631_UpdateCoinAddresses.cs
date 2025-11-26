using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptexAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCoinAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepositAddress",
                table: "Coins",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepositAddress",
                table: "Coins");
        }
    }
}
