using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loyeris.RentCollection.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialRentCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "collection");

            migrationBuilder.CreateTable(
                name: "rent_deadlines",
                schema: "collection",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    lease_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_month = table.Column<DateOnly>(type: "date", nullable: false),
                    due_on = table.Column<DateOnly>(type: "date", nullable: false),
                    rent_excluding_charges_cents = table.Column<long>(type: "bigint", nullable: false),
                    charges_cents = table.Column<long>(type: "bigint", nullable: false),
                    total_due_cents = table.Column<long>(type: "bigint", nullable: false),
                    paid_cents = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    remaining_cents = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Pending"),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rent_deadlines", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rent_payments",
                schema: "collection",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rent_deadline_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_cents = table.Column<long>(type: "bigint", nullable: false),
                    paid_on = table.Column<DateOnly>(type: "date", nullable: false),
                    recorded_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    reference = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rent_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_rent_payments_rent_deadlines_rent_deadline_id",
                        column: x => x.rent_deadline_id,
                        principalSchema: "collection",
                        principalTable: "rent_deadlines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rent_reminders",
                schema: "collection",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rent_deadline_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Scheduled"),
                    scheduled_for = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    recipient_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    body = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rent_reminders", x => x.id);
                    table.ForeignKey(
                        name: "FK_rent_reminders_rent_deadlines_rent_deadline_id",
                        column: x => x.rent_deadline_id,
                        principalSchema: "collection",
                        principalTable: "rent_deadlines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rent_deadlines_due_on_status",
                schema: "collection",
                table: "rent_deadlines",
                columns: new[] { "due_on", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_rent_deadlines_lease_id_period_month",
                schema: "collection",
                table: "rent_deadlines",
                columns: new[] { "lease_id", "period_month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rent_deadlines_period_month_status",
                schema: "collection",
                table: "rent_deadlines",
                columns: new[] { "period_month", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_rent_payments_paid_on",
                schema: "collection",
                table: "rent_payments",
                column: "paid_on",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_rent_payments_rent_deadline_id_paid_on",
                schema: "collection",
                table: "rent_payments",
                columns: new[] { "rent_deadline_id", "paid_on" });

            migrationBuilder.CreateIndex(
                name: "IX_rent_reminders_rent_deadline_id",
                schema: "collection",
                table: "rent_reminders",
                column: "rent_deadline_id");

            migrationBuilder.CreateIndex(
                name: "IX_rent_reminders_status_scheduled_for",
                schema: "collection",
                table: "rent_reminders",
                columns: new[] { "status", "scheduled_for" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rent_payments",
                schema: "collection");

            migrationBuilder.DropTable(
                name: "rent_reminders",
                schema: "collection");

            migrationBuilder.DropTable(
                name: "rent_deadlines",
                schema: "collection");
        }
    }
}
