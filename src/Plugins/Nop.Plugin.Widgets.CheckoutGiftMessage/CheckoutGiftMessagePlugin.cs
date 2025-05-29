using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.CheckoutGiftMessage.Components;
using Nop.Services.Attributes;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage
{
    public class CheckoutGiftMessagePlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
        private readonly ISettingService _settingService;

        public CheckoutGiftMessagePlugin(IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService, ISettingService settingService)
        {
            _checkoutAttributeService = checkoutAttributeService;
            _settingService = settingService;
        }

        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(WidgetGiftMessageViewComponent);
        }

        public async Task<IList<string>> GetWidgetZonesAsync()
        {
            List<string> widgetZones = new List<string>
            {
                PublicWidgetZones.OrderSummaryCartFooter
            };

            return widgetZones;
        }

        public override async Task InstallAsync()
        {
            CheckoutAttribute giftMessageCheckoutAttribute = new CheckoutAttribute
            {
                Name = "Gift Message",
                TextPrompt = "Enter your gift message here",
                AttributeControlType = AttributeControlType.MultilineTextbox,
                DisplayOrder = 1,
            };

            await _checkoutAttributeService.InsertAttributeAsync(giftMessageCheckoutAttribute);

            //save gift message checkout attribute in settings
            await _settingService.SetSettingAsync(Defaults.GIFT_CARD_CHECKOUT_ATTRIBUTE_SETTING_KEY, giftMessageCheckoutAttribute.Id);

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            int giftMessageCheckoutAttributeId = await _settingService.GetSettingByKeyAsync<int>(Defaults.GIFT_CARD_CHECKOUT_ATTRIBUTE_SETTING_KEY);

            if (giftMessageCheckoutAttributeId == 0)
                return;

            var giftMessageCheckoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(giftMessageCheckoutAttributeId);

            if (giftMessageCheckoutAttribute == null)
                return;

            await _checkoutAttributeService.DeleteAttributeAsync(giftMessageCheckoutAttribute);

            await base.UninstallAsync();
        }
    }
}