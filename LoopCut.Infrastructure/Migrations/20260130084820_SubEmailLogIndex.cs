using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoopCut.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SubEmailLogIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // 2. Tạo hàm Immutable để ép kiểu date trong Index
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION get_date_immutable(ts timestamptz) 
                RETURNS date AS $$
                BEGIN
                    RETURN ts::date;
                END;
                $$ LANGUAGE plpgsql IMMUTABLE;
            ");

            // 3. Tạo Index cho Daily Reminder (Type = 2) - 1 mail/ngày
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IF NOT EXISTS ""UX_EmailLog_DailyReminder""
                ON ""SubscriptionEmailLogs"" (
                    ""SubscriptionId"",
                    ""Type"",
                    (get_date_immutable(""SentAt""))
                )
                WHERE ""Type"" = 2;
            ");

            // 4. Tạo Index cho Pre-reminder (Type = 1) - Duy nhất 1 lần
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IF NOT EXISTS ""UX_EmailLog_PreReminder""
                ON ""SubscriptionEmailLogs"" (
                    ""SubscriptionId"",
                    ""Type""
                )
                WHERE ""Type"" = 1;
            ");

            // 5. Tạo Index cho Expired (Type = 3) - Duy nhất 1 lần
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IF NOT EXISTS ""UX_EmailLog_Expired""
                ON ""SubscriptionEmailLogs"" (
                    ""SubscriptionId"",
                    ""Type""
                )
                WHERE ""Type"" = 3;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa Index khi Rollback
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"UX_EmailLog_DailyReminder\";");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"UX_EmailLog_PreReminder\";");
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"UX_EmailLog_Expired\";");

            // Xóa hàm 
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS get_date_immutable(timestamptz);");
        }
    }
}
