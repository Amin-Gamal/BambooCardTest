using Nop.Services.Cms;
using Nop.Services.Plugins;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage
{
    public class CheckoutGiftMessagePlugin : BasePlugin, IWidgetPlugin
    {
        public CheckoutGiftMessagePlugin()
        {
        }

        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            throw new NotImplementedException();
        }
    }
}