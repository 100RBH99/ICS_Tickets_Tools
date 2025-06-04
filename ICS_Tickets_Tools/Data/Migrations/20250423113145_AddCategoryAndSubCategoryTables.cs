using Microsoft.EntityFrameworkCore.Migrations;

namespace ICS_Tickets_Tools.Data.Migrations
{
    public partial class AddCategoryAndSubCategoryTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SubCategory",
                table: "Tickets_Tbl",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(Max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Tickets_Tbl",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(Max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Category_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category_Tbl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubCategory_Tbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategory_Tbl", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Category_Tbl");

            migrationBuilder.DropTable(
                name: "SubCategory_Tbl");

            migrationBuilder.AlterColumn<string>(
                name: "SubCategory",
                table: "Tickets_Tbl",
                type: "nvarchar(Max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Tickets_Tbl",
                type: "nvarchar(Max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
