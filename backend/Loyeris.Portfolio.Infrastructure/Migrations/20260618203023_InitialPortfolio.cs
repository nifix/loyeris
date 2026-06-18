using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loyeris.Portfolio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "portfolio");

            migrationBuilder.CreateTable(
                name: "scis",
                schema: "portfolio",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    siren = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    tax_regime = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "IR"),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    street = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    city = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, defaultValue: "FR"),
                    incorporated_on = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    archived_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scis", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lots",
                schema: "portfolio",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sci_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    street = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    city = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false, defaultValue: "FR"),
                    surface_sqm = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: true),
                    potential_rent_excluding_charges_cents = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    potential_charges_cents = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    suggested_deposit_cents = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    archived_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lots", x => x.id);
                    table.ForeignKey(
                        name: "FK_lots_scis_sci_id",
                        column: x => x.sci_id,
                        principalSchema: "portfolio",
                        principalTable: "scis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sci_associates",
                schema: "portfolio",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sci_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    last_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    shares_count = table.Column<int>(type: "integer", nullable: true),
                    ownership_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sci_associates", x => x.id);
                    table.ForeignKey(
                        name: "FK_sci_associates_scis_sci_id",
                        column: x => x.sci_id,
                        principalSchema: "portfolio",
                        principalTable: "scis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lots_sci_id_reference",
                schema: "portfolio",
                table: "lots",
                columns: new[] { "sci_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lots_sci_id_status",
                schema: "portfolio",
                table: "lots",
                columns: new[] { "sci_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_sci_associates_sci_id",
                schema: "portfolio",
                table: "sci_associates",
                column: "sci_id");

            migrationBuilder.CreateIndex(
                name: "IX_scis_siren",
                schema: "portfolio",
                table: "scis",
                column: "siren",
                unique: true,
                filter: "siren IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_scis_workspace_id_name",
                schema: "portfolio",
                table: "scis",
                columns: new[] { "workspace_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_scis_workspace_id_status",
                schema: "portfolio",
                table: "scis",
                columns: new[] { "workspace_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lots",
                schema: "portfolio");

            migrationBuilder.DropTable(
                name: "sci_associates",
                schema: "portfolio");

            migrationBuilder.DropTable(
                name: "scis",
                schema: "portfolio");
        }
    }
}
