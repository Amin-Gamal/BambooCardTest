using Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Models;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Factories
{
    public interface IBambooProductAttributeModelFactory
    {
        Task<ProductAttributeListModel> PrepareProductAttributeListModelAsync(ProductAttributeSearchModel searchModel);
    }
}