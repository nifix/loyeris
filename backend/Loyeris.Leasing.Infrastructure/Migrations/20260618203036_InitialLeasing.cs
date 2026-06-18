using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loyeris.Leasing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialLeasing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "leasing");

            migrationBuilder.CreateTable(
                name: "leases",
                schema: "leasing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Draft"),
                    starts_on = table.Column<DateOnly>(type: "date", nullable: false),
                    ends_on = table.Column<DateOnly>(type: "date", nullable: true),
                    rent_due_day = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    rent_excluding_charges_cents = table.Column<long>(type: "bigint", nullable: false),
                    charges_cents = table.Column<long>(type: "bigint", nullable: false),
                    deposit_cents = table.Column<long>(type: "bigint", nullable: false),
                    payment_terms = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "leasing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    last_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    phone = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    archived_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lease_tenants",
                schema: "leasing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lease_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Primary"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lease_tenants", x => x.id);
                    table.ForeignKey(
                        name: "FK_lease_tenants_leases_lease_id",
                        column: x => x.lease_id,
                        principalSchema: "leasing",
                        principalTable: "leases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lease_tenants_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalSchema: "leasing",
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lease_tenants_lease_id_tenant_id",
                schema: "leasing",
                table: "lease_tenants",
                columns: new[] { "lease_id", "tenant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_lease_tenants_tenant_id",
                schema: "leasing",
                table: "lease_tenants",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_leases_lot_id",
                schema: "leasing",
                table: "leases",
                column: "lot_id",
                unique: true,
                filter: "status = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_leases_lot_id_status",
                schema: "leasing",
                table: "leases",
                columns: new[] { "lot_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_leases_starts_on_ends_on",
                schema: "leasing",
                table: "leases",
                columns: new[] { "starts_on", "ends_on" });

            migrationBuilder.CreateIndex(
                name: "IX_tenants_workspace_id_email",
                schema: "leasing",
                table: "tenants",
                columns: new[] { "workspace_id", "email" },
                unique: true,
                filter: "email IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_workspace_id_status_last_name",
                schema: "leasing",
                table: "tenants",
                columns: new[] { "workspace_id", "status", "last_name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lease_tenants",
                schema: "leasing");

            migrationBuilder.DropTable(
                name: "leases",
                schema: "leasing");

            migrationBuilder.DropTable(
                name: "tenants",
                schema: "leasing");
        }
    }
}
