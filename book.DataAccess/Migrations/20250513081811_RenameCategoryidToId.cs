using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace book.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameCategoryidToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename column 'Categoryid' to 'Id' in 'Categories' table
            migrationBuilder.RenameColumn(
                name: "Categoryid",   // Old column name
                table: "Categories",  // Table name
                newName: "Id");       // New column name
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the column name from 'Id' back to 'Categoryid' in case of rollback
            migrationBuilder.RenameColumn(
                name: "Id",           // New column name
                table: "Categories",  // Table name
                newName: "Categoryid"); // Old column name
        }
    }
}
