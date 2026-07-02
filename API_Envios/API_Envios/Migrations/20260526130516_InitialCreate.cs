using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API_Envios.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Nit = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Distrito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pilotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Distrito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pilotos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Distrito = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DireccionCercana = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpresaId = table.Column<int>(type: "int", nullable: true),
                    PilotoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Destinatarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Distrito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinatarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destinatarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Envios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoRastreo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    DestinatarioId = table.Column<int>(type: "int", nullable: false),
                    PilotoId = table.Column<int>(type: "int", nullable: true),
                    DescripcionPedido = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PesoLibras = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirmaRecibido = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ImagenesEntrega = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Envios_Destinatarios_DestinatarioId",
                        column: x => x.DestinatarioId,
                        principalTable: "Destinatarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Envios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Envios_Pilotos_PilotoId",
                        column: x => x.PilotoId,
                        principalTable: "Pilotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEnvios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EnvioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEnvios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEnvios_Envios_EnvioId",
                        column: x => x.EnvioId,
                        principalTable: "Envios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Empresas",
                columns: new[] { "Id", "Activa", "Correo", "Departamento", "Direccion", "Distrito", "Nit", "Nombre", "Telefono" },
                values: new object[] { 1, true, "empresa@envios.test", "San Salvador", "Colonia Escalon", "San Salvador Centro", "0614-260526-101-1", "Mishi Store", "77777777" });

            migrationBuilder.InsertData(
                table: "Pilotos",
                columns: new[] { "Id", "Activo", "Departamento", "Distrito", "Documento", "Nombre", "Telefono" },
                values: new object[] { 1, true, "San Salvador", "San Salvador Centro", "00000000-0", "Piloto Demo", "77770000" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Correo", "Departamento", "DireccionCercana", "Distrito", "EmpresaId", "Nombre", "Password", "PilotoId", "Rol" },
                values: new object[,]
                {
                    { 1, true, "master@envios.test", null, null, null, null, "Administrador Master", "Master123", null, "AdministradorMaster" },
                    { 2, true, "admin@envios.test", "San Salvador", "Centro", "San Salvador Centro", null, "Admin San Salvador", "Admin123", null, "Administrador" },
                    { 3, true, "empresa@envios.test", null, null, null, 1, "Mishi Store", "Empresa123", null, "EmpresaCliente" },
                    { 4, true, "piloto@envios.test", null, null, null, null, "Piloto Demo", "Piloto123", 1, "Piloto" }
                });

            migrationBuilder.InsertData(
                table: "Destinatarios",
                columns: new[] { "Id", "Departamento", "Direccion", "Distrito", "EmpresaId", "Nombre", "Telefono" },
                values: new object[] { 1, "San Salvador", "San Salvador", "San Salvador Centro", 1, "Michelle Jimenez", "77777777" });

            migrationBuilder.InsertData(
                table: "Envios",
                columns: new[] { "Id", "CodigoRastreo", "DescripcionPedido", "DestinatarioId", "EmpresaId", "Estado", "FechaCreacion", "FechaEntrega", "FirmaRecibido", "ImagenesEntrega", "PesoLibras", "PilotoId" },
                values: new object[] { 1, "ENV-20260526-0001", "Paquete de prueba", 1, 1, "EnRuta", new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "[]", 2.5m, 1 });

            migrationBuilder.InsertData(
                table: "HistorialEnvios",
                columns: new[] { "Id", "Comentario", "EnvioId", "Estado", "Fecha", "Usuario" },
                values: new object[,]
                {
                    { 1, "Envio creado", 1, "Recolectado", new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), "Sistema" },
                    { 2, "Asignado a piloto demo", 1, "EnRuta", new DateTime(2026, 5, 26, 0, 5, 0, 0, DateTimeKind.Utc), "Administrador Master" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destinatarios_EmpresaId",
                table: "Destinatarios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_CodigoRastreo",
                table: "Envios",
                column: "CodigoRastreo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Envios_DestinatarioId",
                table: "Envios",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_EmpresaId",
                table: "Envios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Envios_PilotoId",
                table: "Envios",
                column: "PilotoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEnvios_EnvioId",
                table: "HistorialEnvios",
                column: "EnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialEnvios");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Envios");

            migrationBuilder.DropTable(
                name: "Destinatarios");

            migrationBuilder.DropTable(
                name: "Pilotos");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
