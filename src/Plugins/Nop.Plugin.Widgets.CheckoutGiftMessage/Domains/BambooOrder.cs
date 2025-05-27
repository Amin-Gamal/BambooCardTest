using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage.Domains
{
    public class BambooOrder : Order
    {
        public string GiftMessage { get; set; }
    }
}