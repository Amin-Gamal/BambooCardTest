using FluentMigrator;
using Nop.Core.Domain.Orders;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.CheckoutGiftMessage.Domains;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage.Data.Migrations
{
    [NopMigration("2025/05/27 17:20:00", "Add Gift message column to order table", MigrationProcessType.Installation)]
    public class AddGiftMessageColumnToOrderMigration : Migration
    {
        public override void Down()
        {
            if (Schema.Table(nameof(Order)).Column(nameof(BambooOrder.GiftMessage)).Exists())
                Delete.Column(nameof(BambooOrder.GiftMessage)).FromTable(nameof(Order));
        }

        public override void Up()
        {
            if (!Schema.Table(nameof(Order)).Column(nameof(BambooOrder.GiftMessage)).Exists())
                Alter.Table(nameof(Order)).AddColumn(nameof(BambooOrder.GiftMessage)).AsString(int.MaxValue).Nullable();
        }
    }
}