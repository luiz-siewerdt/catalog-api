using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogApi.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryCaseSensetiveInName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "text",
                nullable: false,
                collation: "pt_BR.utf8",
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "pt_BR.utf8");
        }
    }
}
