using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

using Nop.Services.Attributes;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage
{
    public class CheckoutGiftMessagePlugin : BasePlugin
    {
        private readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;

        public CheckoutGiftMessagePlugin(IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService, 
                                         ISettingService settingService,
                                         ILocalizationService localizationService)
        {
            _checkoutAttributeService = checkoutAttributeService;
            _settingService = settingService;
            _localizationService = localizationService;
        }

        public override async Task InstallAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.CheckoutGiftMessage.EnterGiftMessage"] = "Enter Your gift's message",
                ["Plugins.Widgets.CheckoutGiftMessage.GiftMessage"] = "Gift Message",
                
            });

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
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Widgets.CheckoutGiftMessage");

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