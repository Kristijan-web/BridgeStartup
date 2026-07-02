using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Access.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredRoleUseCasesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "RoleUseCases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "RoleUseCases",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
