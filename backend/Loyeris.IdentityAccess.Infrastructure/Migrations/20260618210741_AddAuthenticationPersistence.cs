using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loyeris.IdentityAccess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "access_failed_count",
                schema: "identity",
                table: "app_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "email_confirmed_at",
                schema: "identity",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "lockout_ends_at",
                schema: "identity",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "password_changed_at",
                schema: "identity",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "security_stamp",
                schema: "identity",
                table: "app_users",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValueSql: "gen_random_uuid()::text");

            migrationBuilder.CreateTable(
                name: "auth_one_time_tokens",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purpose = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    sent_to_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    consumed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    requested_by_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_one_time_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_auth_one_time_tokens_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auth_sessions",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    device_label = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    last_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_reason = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_auth_sessions_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "auth_events",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    metadata_json = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auth_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_auth_events_app_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_auth_events_auth_sessions_session_id",
                        column: x => x.session_id,
                        principalSchema: "identity",
                        principalTable: "auth_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    consumed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_reason = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    replaced_by_token_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    consumed_by_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_auth_sessions_session_id",
                        column: x => x.session_id,
                        principalSchema: "identity",
                        principalTable: "auth_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_refresh_tokens_replaced_by_token_id",
                        column: x => x.replaced_by_token_id,
                        principalSchema: "identity",
                        principalTable: "refresh_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auth_events_normalized_email_occurred_at",
                schema: "identity",
                table: "auth_events",
                columns: new[] { "normalized_email", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "IX_auth_events_session_id",
                schema: "identity",
                table: "auth_events",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_auth_events_type_occurred_at",
                schema: "identity",
                table: "auth_events",
                columns: new[] { "type", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "IX_auth_events_user_id_occurred_at",
                schema: "identity",
                table: "auth_events",
                columns: new[] { "user_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "IX_auth_one_time_tokens_expires_at",
                schema: "identity",
                table: "auth_one_time_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_auth_one_time_tokens_token_hash",
                schema: "identity",
                table: "auth_one_time_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auth_one_time_tokens_user_id_purpose",
                schema: "identity",
                table: "auth_one_time_tokens",
                columns: new[] { "user_id", "purpose" },
                unique: true,
                filter: "consumed_at IS NULL AND revoked_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_auth_sessions_status_expires_at",
                schema: "identity",
                table: "auth_sessions",
                columns: new[] { "status", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_auth_sessions_user_id_status_last_seen_at",
                schema: "identity",
                table: "auth_sessions",
                columns: new[] { "user_id", "status", "last_seen_at" });

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_expires_at",
                schema: "identity",
                table: "refresh_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_replaced_by_token_id",
                schema: "identity",
                table: "refresh_tokens",
                column: "replaced_by_token_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_session_id",
                schema: "identity",
                table: "refresh_tokens",
                column: "session_id",
                unique: true,
                filter: "consumed_at IS NULL AND revoked_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token_hash",
                schema: "identity",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auth_events",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "auth_one_time_tokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "auth_sessions",
                schema: "identity");

            migrationBuilder.DropColumn(
                name: "access_failed_count",
                schema: "identity",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "email_confirmed_at",
                schema: "identity",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "lockout_ends_at",
                schema: "identity",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "password_changed_at",
                schema: "identity",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "security_stamp",
                schema: "identity",
                table: "app_users");
        }
    }
}
