using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appweb2.Migrations
{
    /// <inheritdoc />
    public partial class inicia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.CheckConstraint("CK_Categorias_Nombre_NoVacio", "LEN(LTRIM(RTRIM([Nombre]))) > 0");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoJuegos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrecioPromocion = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Imagen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnPromocion = table.Column<bool>(type: "bit", nullable: false),
                    EdadMinima = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoJuegos", x => x.Id);
                    table.CheckConstraint("CK_VideoJuegos_EdadMinima_Valida", "[EdadMinima] BETWEEN 0 AND 21");
                    table.CheckConstraint("CK_VideoJuegos_Precio_MayorACero", "[Precio] > 0");
                    table.CheckConstraint("CK_VideoJuegos_PrecioPromocion_Valido", "[PrecioPromocion] IS NULL OR ([PrecioPromocion] > 0 AND [PrecioPromocion] < [Precio])");
                    table.ForeignKey(
                        name: "FK_VideoJuegos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(103)", maxLength: 103, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(153)", maxLength: 153, nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    VideoJuegoID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Compras_VideoJuegos_VideoJuegoID",
                        column: x => x.VideoJuegoID,
                        principalTable: "VideoJuegos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detalle_compra",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoJuegosId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    estadoCompra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    fechaHoraTransaccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    codigoTransaccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    idCompra = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalle_compra", x => x.id);
                    table.CheckConstraint("CK_DetalleCompra_Cantidad_Positiva", "[cantidad] > 0");
                    table.CheckConstraint("CK_DetalleCompra_Total_NoNegativo", "[total] >= 0");
                    table.ForeignKey(
                        name: "FK_detalle_compra_Compras_idCompra",
                        column: x => x.idCompra,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Compras_UsuarioID",
                table: "Compras",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_VideoJuegoID",
                table: "Compras",
                column: "VideoJuegoID");

            migrationBuilder.CreateIndex(
                name: "IX_detalle_compra_idCompra",
                table: "detalle_compra",
                column: "idCompra");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RoleId",
                table: "Usuarios",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoJuegos_CategoriaId",
                table: "VideoJuegos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalle_compra");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "VideoJuegos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
