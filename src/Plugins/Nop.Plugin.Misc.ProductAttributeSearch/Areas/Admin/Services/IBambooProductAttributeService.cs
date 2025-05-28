using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Services
{
    public interface IBambooProductAttributeService
    {
        Task<IPagedList<ProductAttribute>> GetAllProductAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue, string productAttributeName = "");
    }
}