using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.BambooCardDiscounts.Services;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class BambooCardDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        private readonly IBambooCardDiscountsService _bambooCardDiscountsService;
        private readonly IDiscountService _discountService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public BambooCardDiscountRequirementRule(IBambooCardDiscountsService bambooCardDiscountsService,
                                         IDiscountService discountService,
                                         ISettingService settingService,

                                         ILocalizationService localizationService,
                                         IWebHelper webHelper,

                                         IUrlHelperFactory urlHelperFactory,
                                         IActionContextAccessor actionContextAccessor)
        {
            _bambooCardDiscountsService = bambooCardDiscountsService;
            _discountService = discountService;
            _settingService = settingService;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Customer == null)
                return new DiscountRequirementValidationResult();

            int requiredNumberOfOrders = await _settingService.GetSettingByKeyAsync<int>(string.Format(Defaults.BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SETTING_KEY, request.DiscountRequirementId));
            if (requiredNumberOfOrders == 0)
                return new DiscountRequirementValidationResult();

            int numberOfOrders = await _bambooCardDiscountsService.GetNumberOfCompletedOrdersByCustomerIdAsync(request.Customer.Id);

            if (numberOfOrders < requiredNumberOfOrders)
                return new DiscountRequirementValidationResult();

            return new DiscountRequirementValidationResult { IsValid = true };
        }

        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            return urlHelper.Action("Configure", "BambooCardDiscounts",
                new { discountId = discountId, discountRequirementId = discountRequirementId }, _webHelper.GetCurrentRequestProtocol());
        }

        public override async Task InstallAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.DiscountRules.BambooCardDiscounts.Fields.MinimumNumberOfOrders"] = "Minimum No. Of Orders"
            });

            Discount discount = await CheckIfDiscountInstalled();

            if (discount != null)
                return; //already installed

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

            //add default requirement group
            var defaultGroup = new DiscountRequirement
            {
                IsGroup = true,
                DiscountId = BambooCardDiscount.Id,
                InteractionType = RequirementGroupInteractionType.And,
                DiscountRequirementRuleSystemName = await _localizationService
                    .GetResourceAsync("Admin.Promotions.Discounts.Requirements.DefaultRequirementGroup")
            };

            await _discountService.InsertDiscountRequirementAsync(defaultGroup);

            //create discount requirement
            DiscountRequirement discountRequirement = new DiscountRequirement()
            {
                DiscountId = BambooCardDiscount.Id,
                DiscountRequirementRuleSystemName = Defaults.BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SYSTEM_NAME,
                ParentId = defaultGroup.Id,
            };

            await _discountService.InsertDiscountRequirementAsync(discountRequirement);

            //save minimum number of orders elegible for discount into settings
            await _settingService.SetSettingAsync(string.Format(Defaults.BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SETTING_KEY, discountRequirement.Id), Defaults.MINIMUM_NUMBER_OF_ORDERS_DEFAULT_VALUE);

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.DiscountRules.BambooCardDiscounts");

            Discount discount = await CheckIfDiscountInstalled();

            if (discount == null)
                return;

            //delete discount
            await _discountService.DeleteDiscountAsync(discount);

            //delete discount setting
            Setting discountIdSetting = await _settingService.GetSettingAsync(Defaults.BAMBOO_CARD_DISCOUNTS_SETTING_KEY);
            if (discountIdSetting != null)
                await _settingService.DeleteSettingAsync(discountIdSetting);

            //delete discount requirement setting
            string settingKeyPrefix = Defaults.BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SETTING_KEY.Split('-').FirstOrDefault().ToLower();
            List<Setting> discountRequirementSettings = _settingService.GetAllSettings().Where(s => s.Name.StartsWith(settingKeyPrefix)).ToList();

            if (discountRequirementSettings != null && discountRequirementSettings.Any())
                await _settingService.DeleteSettingsAsync(discountRequirementSettings);

            await base.UninstallAsync();
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