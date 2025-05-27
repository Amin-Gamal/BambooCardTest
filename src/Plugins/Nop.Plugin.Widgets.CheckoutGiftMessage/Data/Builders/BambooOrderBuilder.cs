using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Widgets.CheckoutGiftMessage.Domains;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage.Data.Builders
{
    internal class BambooOrderBuilder : NopEntityBuilder<BambooOrder>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table.WithColumn(nameof(BambooOrder.GiftMessage)).AsString(int.MaxValue).Nullable();
        }
    }
}