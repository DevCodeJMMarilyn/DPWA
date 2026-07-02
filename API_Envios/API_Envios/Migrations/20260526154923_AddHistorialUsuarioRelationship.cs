using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Envios.Migrations
{
    /// <inheritdoc />
    public partial class AddHistorialUsuarioRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EnvioId",
                table: "HistorialEnvios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "HistorialEnvios",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "HistorialEnvios",
                keyColumn: "Id",
                keyValue: 1,
                column: "UsuarioId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "HistorialEnvios",
                keyColumn: "Id",
                keyValue: 2,
                column: "UsuarioId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEnvios_UsuarioId",
                table: "HistorialEnvios",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialEnvios_Usuarios_UsuarioId",
                table: "HistorialEnvios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialEnvios_Usuarios_UsuarioId",
                table: "HistorialEnvios");

            migrationBuilder.DropIndex(
                name: "IX_HistorialEnvios_UsuarioId",
                table: "HistorialEnvios");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "HistorialEnvios");

            migrationBuilder.AlterColumn<int>(
                name: "EnvioId",
                table: "HistorialEnvios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
