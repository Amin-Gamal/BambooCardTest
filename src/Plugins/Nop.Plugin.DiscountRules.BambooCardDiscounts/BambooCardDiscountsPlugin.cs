using Nop.Core.Domain.Discounts;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Plugins;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class BambooCardDiscountsPlugin : BasePlugin
    {
        private readonly IDiscountService _discountService;
        private readonly ISettingService _settingService;

        public BambooCardDiscountsPlugin(IDiscountService discountService,
                                         ISettingService settingService
                                         )
        {
            _discountService = discountService;
            _settingService = settingService;
        }

        public override async Task InstallAsync()
        {
            Discount discount = await CheckIfDiscountInstalled();

            if (discount != null)
            {
                //already installed
                return;
            }

            Discount BambooCardDiscount = new Discount()
            {
                Name = Defaults.BAMBOO_CARD_DISCOUNTS_NAME,
                DiscountTypeId = (int)DiscountType.AssignedToOrderTotal,
                UsePercentage = true,
                DiscountPercentage = 10,
                RequiresCouponCode = false,
                IsActive = true,
                DiscountLimitation = DiscountLimitationType.Unlimited,
            };

            await _discountService.InsertDiscountAsync(BambooCardDiscount);

            //save discount id to setting
            await _settingService.SetSettingAsync(Defaults.BAMBOO_CARD_DISCOUNTS_SETTING_KEY, string.Format(Defaults.BAMBOO_CARD_DISCOUNTS_SETTING_VALUE, BambooCardDiscount.Id));

            await base.InstallAsync();
        }

        /// <summary>
        /// returns Discount Object from the setting value
        /// </summary>
        /// <returns> null if Discount is not installed otherwise returns discount object</returns>
        protected async Task<Discount> CheckIfDiscountInstalled()
        {
            string setting = await _settingService.GetSettingByKeyAsync<string>(Defaults.BAMBOO_CARD_DISCOUNTS_SETTING_KEY);

            if (string.IsNullOrEmpty(setting))
                return null;

            int.TryParse(setting?.Split('-').LastOrDefault(), out int discountId);

            if (discountId == 0)
                return null;

            Discount discount = await _discountService.GetDiscountByIdAsync(discountId);

            return discount;
        }
    }
}