using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.ProductAttributeSearch
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class ProductAttributeSearchPlugin : BasePlugin
    {
        private readonly ILocalizationService _localizationService;

        public ProductAttributeSearchPlugin(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public override async Task InstallAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync("Plugins.Misc.ProductAttributeSearch.Fields.SearchProductAttributeName", "Name");

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Misc.ProductAttributeSearch");

            await base.UninstallAsync();
        }
    }
}