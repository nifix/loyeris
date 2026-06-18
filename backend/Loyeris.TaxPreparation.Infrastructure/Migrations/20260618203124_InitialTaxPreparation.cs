using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loyeris.TaxPreparation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialTaxPreparation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tax");

            migrationBuilder.CreateTable(
                name: "fiscal_periods",
                schema: "tax",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sci_id = table.Column<Guid>(type: "uuid", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    starts_on = table.Column<DateOnly>(type: "date", nullable: false),
                    ends_on = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Open"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fiscal_periods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rental_expenses",
                schema: "tax",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sci_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_id = table.Column<Guid>(type: "uuid", nullable: true),
                    expense_date = table.Column<DateOnly>(type: "date", nullable: false),
                    category = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    label = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    amount_cents = table.Column<long>(type: "bigint", nullable: false),
                    deductible_for_ir = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    document_url = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rental_expenses", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fiscal_periods_sci_id_year",
                schema: "tax",
                table: "fiscal_periods",
                columns: new[] { "sci_id", "year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rental_expenses_lot_id_expense_date",
                schema: "tax",
                table: "rental_expenses",
                columns: new[] { "lot_id", "expense_date" });

            migrationBuilder.CreateIndex(
                name: "IX_rental_expenses_sci_id_expense_date",
                schema: "tax",
                table: "rental_expenses",
                columns: new[] { "sci_id", "expense_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fiscal_periods",
                schema: "tax");

            migrationBuilder.DropTable(
                name: "rental_expenses",
                schema: "tax");
        }
    }
}
