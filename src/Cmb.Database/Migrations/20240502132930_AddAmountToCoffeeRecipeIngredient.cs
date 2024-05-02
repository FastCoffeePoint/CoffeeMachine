using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cmb.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountToCoffeeRecipeIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "CoffeeRecipeIngredients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "CoffeeRecipeIngredients");
        }
    }
}
