using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoopCut.Infrastructure.Migrations
{
    public partial class EmailIndexLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Thêm một Computed Column để trích xuất Ngày từ SentAt (Để tạo Unique Index theo ngày)
            // MS SQL không cho phép dùng trực tiếp hàm CAST/CONVERT trong INDEX nếu không qua Computed Column
            migrationBuilder.Sql(@"
                ALTER TABLE [SubscriptionEmailLogs] 
                ADD [SentDate] AS CAST([SentAt] AS DATE) PERSISTED;
            ");

            // 2. Tạo Index cho Daily Reminder (Type = 2) - 1 mail/ngày
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX [UX_EmailLog_DailyReminder]
                ON [SubscriptionEmailLogs] ([SubscriptionId], [Type], [SentDate])
                WHERE [Type] = 2;
            ");

            // 3. Tạo Index cho Pre-reminder (Type = 1) - Duy nhất 1 lần
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX [UX_EmailLog_PreReminder]
                ON [SubscriptionEmailLogs] ([SubscriptionId], [Type])
                WHERE [Type] = 1;
            ");

            // 4. Tạo Index cho Expired (Type = 3) - Duy nhất 1 lần
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX [UX_EmailLog_Expired]
                ON [SubscriptionEmailLogs] ([SubscriptionId], [Type])
                WHERE [Type] = 3;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa Index
            migrationBuilder.Sql("DROP INDEX [UX_EmailLog_DailyReminder] ON [SubscriptionEmailLogs];");
            migrationBuilder.Sql("DROP INDEX [UX_EmailLog_PreReminder] ON [SubscriptionEmailLogs];");
            migrationBuilder.Sql("DROP INDEX [UX_EmailLog_Expired] ON [SubscriptionEmailLogs];");

            // Xóa cột tính toán
            migrationBuilder.Sql("ALTER TABLE [SubscriptionEmailLogs] DROP COLUMN [SentDate];");
        }
    }
}