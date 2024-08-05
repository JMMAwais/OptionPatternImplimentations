using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OptionPatternImplimentations.Migrations
{
    /// <inheritdoc />
    public partial class changeUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "users",
                newName: "passwor");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "users",
                newName: "email");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "passwor",
                table: "users",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "UserName");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
