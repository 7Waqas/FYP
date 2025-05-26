using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateStatusToOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdateStatus",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateStatus",
                table: "OrderHeaders");
        }
    }
}
