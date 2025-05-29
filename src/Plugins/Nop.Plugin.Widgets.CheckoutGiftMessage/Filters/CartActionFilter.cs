using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Services.Configuration;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage.Filters
{
    public class CartActionFilter : ActionFilterAttribute
    {
        private readonly ISettingService _settingService;

        public CartActionFilter(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor actionDescriptor))
                return;

            if (actionDescriptor.ControllerTypeInfo != typeof(Nop.Web.Controllers.ShoppingCartController) ||
                actionDescriptor.ActionName != nameof(Nop.Web.Controllers.ShoppingCartController.Cart) || context.HttpContext.Request.Method != "GET")
            {
                return;
            }

            ViewResult viewResult = context.Result as ViewResult;

            if (viewResult is null)
                return;

            ShoppingCartModel model = viewResult.Model as ShoppingCartModel;

            if (model is null || model.CheckoutAttributes == null || model.CheckoutAttributes.Any() == false)
                return;

            // Find the gift message checkout attribute and then remove it from the model
            int giftMessageCheckoutAttributeId = _settingService.GetSettingByKey<int>(Defaults.GIFT_CARD_CHECKOUT_ATTRIBUTE_SETTING_KEY);

            if (giftMessageCheckoutAttributeId == 0)
                return;

            var giftMessageAttribute = model.CheckoutAttributes.FirstOrDefault(x => x.Id == giftMessageCheckoutAttributeId);

            if (giftMessageAttribute == null)
                return;

            model.CheckoutAttributes.Remove(giftMessageAttribute);

            base.OnActionExecuted(context);
        }
    }
}