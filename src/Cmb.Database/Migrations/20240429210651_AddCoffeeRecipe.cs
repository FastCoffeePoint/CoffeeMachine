using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cmb.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCoffeeRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Ingredients",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Ingredients",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CoffeeRecipes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    CreateDate = table.Column<long>(type: "INTEGER", nullable: false),
                    DeleteDate = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeRecipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeRecipeIngredients",
                columns: table => new
                {
                    CoffeeRecipeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IngredientId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeRecipeIngredients", x => new { x.CoffeeRecipeId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_CoffeeRecipeIngredients_CoffeeRecipes_CoffeeRecipeId",
                        column: x => x.CoffeeRecipeId,
                        principalTable: "CoffeeRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoffeeRecipeIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeRecipeIngredients_IngredientId",
                table: "CoffeeRecipeIngredients",
                column: "IngredientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoffeeRecipeIngredients");

            migrationBuilder.DropTable(
                name: "CoffeeRecipes");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Ingredients");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Ingredients",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
