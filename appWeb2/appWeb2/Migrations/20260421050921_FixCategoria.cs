using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appWeb2.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "categoria",
                table: "VideoJuegos",
                newName: "idcategoria");

            migrationBuilder.AlterColumn<string>(
                name: "titulo",
                table: "VideoJuegos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "descripcion",
                table: "VideoJuegos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    idcategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoria = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.idcategoria);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.RenameColumn(
                name: "idcategoria",
                table: "VideoJuegos",
                newName: "categoria");

            migrationBuilder.AlterColumn<string>(
                name: "titulo",
                table: "VideoJuegos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "descripcion",
                table: "VideoJuegos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            
        }
    }
}
