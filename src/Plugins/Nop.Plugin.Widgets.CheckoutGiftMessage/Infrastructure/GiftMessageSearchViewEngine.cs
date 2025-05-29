using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage.Infrastructure
{
    public class GiftMessageSearchViewEngine : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.ViewName == "OnePageCheckout")
                viewLocations = new[] { "~/Plugins/Widgets.CheckoutGiftMessage/Views/Checkout/OnePageCheckout.cshtml" }.Concat(viewLocations);

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}