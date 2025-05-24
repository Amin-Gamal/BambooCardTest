using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts.Events
{
    public class AdminMenuEventConsumer : IConsumer<AdminMenuCreatedEvent>
    {
        private readonly IPermissionService _permissionService;

        public AdminMenuEventConsumer(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_PLUGINS))
                return;


            eventMessage.RootMenuItem.InsertBefore("Help",
                new AdminMenuItem
                {
                    SystemName = "BambooCardPlugins",
                    Title = "BambooCard Discounts",
                    Url = eventMessage.GetMenuItemUrl("BambooCardDiscounts", "BambooCardDiscount"),
                    IconClass = "far fa-dot-circle",
                    Visible = true,
                });
        }

        
    }
}