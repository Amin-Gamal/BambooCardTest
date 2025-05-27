using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.BambooCardDiscounts.Models;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    public class BambooCardDiscountsController : BasePluginController
    {
        private readonly IDiscountService _discountService;
        private readonly ISettingService _settingService;

        public BambooCardDiscountsController(IDiscountService discountService, ISettingService settingService)
        {
            _discountService = discountService;
            _settingService = settingService;
        }

        [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
        public async Task<IActionResult> Configure(int discountId, int? discountRequirementId)
        {
            Discount discount = await _discountService.GetDiscountByIdAsync(discountId);

            if (discount == null)
                return Content("Discount could not be loaded");

            DiscountRequirement discountRequirement = await _discountService.GetDiscountRequirementByIdAsync(discountRequirementId.GetValueOrDefault());

            if (discountRequirementId.HasValue && discountRequirement == null)
                return Content("Failed to load rule.");

            int requiredNumberOfOrders = 0;
            //Use default value if the requirement ID is not set or is zero
            if (discountRequirementId.HasValue == false || discountRequirementId.Value == 0)
            {
                requiredNumberOfOrders = Defaults.MINIMUM_NUMBER_OF_ORDERS_DEFAULT_VALUE;
            }
            else
            {
                //try to get previously saved required number of orders
                requiredNumberOfOrders = await _settingService.GetSettingByKeyAsync<int>(string.Format(Defaults.BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SETTING_KEY, discountRequirementId.GetValueOrDefault()));
            }

            RequirementModel model = new()
            {
                MinimumNumberOfOrders = requiredNumberOfOrders,
                DiscountId = discount.Id,
                RequirementId = discountRequirement?.Id ?? 0,
            };

            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format(Defaults.HTML_FIELD_PREFIX, discountRequirementId.GetValueOrDefault());

            return View("~/Plugins/DiscountRules.BambooCardDiscounts/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
        public async Task<IActionResult> Configure(RequirementModel model)
        {
            if (ModelState.IsValid == false)
                return Ok(new { Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId);
            if (discount == null)
                return NotFound(new { Errors = new[] { "Discount could not be loaded" } });

            //get the discount requirement
            var discountRequirement = await _discountService.GetDiscountRequirementByIdAsync(model.RequirementId);

            //the discount requirement does not exist, so create a new one
            if (discountRequirement == null)
            {
                discountRequirement = new DiscountRequirement
                {
                    DiscountId = discount.Id,
                    DiscountRequirementRuleSystemName = Defaults.BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SYSTEM_NAME
                };

                await _discountService.InsertDiscountRequirementAsync(discountRequirement);
            }

            //Update the required minimum number of orders settings
            string settingKey = string.Format(Defaults.BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SETTING_KEY, discountRequirement.Id);
            Setting setting = await _settingService.GetSettingAsync(settingKey);

            if (setting == null)
            {
                await _settingService.SetSettingAsync(Defaults.BAMBOO_CARD_DISCOUNTS_SETTING_KEY, string.Format(Defaults.BAMBOO_CARD_DISCOUNTS_SETTING_VALUE, discountRequirement.Id));
            }
            else
            {
                setting.Value = model.MinimumNumberOfOrders.ToString();
                await _settingService.UpdateSettingAsync(setting);
            }

            return Ok(new { NewRequirementId = discountRequirement.Id });
        }

        public async Task<IActionResult> BambooCardDiscount()
        {
            string setting = await _settingService.GetSettingByKeyAsync<string>(Defaults.BAMBOO_CARD_DISCOUNTS_SETTING_KEY);

            if (string.IsNullOrEmpty(setting))
            {
                return Content("BambooCard Discount is not installed.");
            }

            int.TryParse(setting?.Split('-').LastOrDefault(), out int discountId);

            return RedirectToAction("Edit", "Discount", new { id = discountId });
        }
    }
}