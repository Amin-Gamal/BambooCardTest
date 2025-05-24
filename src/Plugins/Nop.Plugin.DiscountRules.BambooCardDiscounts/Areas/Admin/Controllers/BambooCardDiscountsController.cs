using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    public class BambooCardDiscountsController : BasePluginController
    {
        private readonly ISettingService _settingService;

        public BambooCardDiscountsController(ISettingService settingService)
        {
            _settingService = settingService;
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