using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Data;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Services
{
    public class BambooProductAttributeService : IBambooProductAttributeService
    {
        private readonly IRepository<ProductAttribute> _productAttributeRepository;

        public BambooProductAttributeService(IRepository<ProductAttribute> productAttributeRepository)
        {
            _productAttributeRepository = productAttributeRepository;
        }

        public virtual async Task<IPagedList<ProductAttribute>> GetAllProductAttributesAsync(int pageIndex = 0, int pageSize = int.MaxValue, string productAttributeName = "")
        {
            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            var productsAttributesQuery = _productAttributeRepository.Table;

            if (string.IsNullOrEmpty(productAttributeName) == false)
                productsAttributesQuery = productsAttributesQuery.Where(pa => pa.Name.Contains(productAttributeName));

            var productsAttributes = await productsAttributesQuery.OrderBy(pa => pa.Name).ToPagedListAsync(pageIndex, pageSize);

            return new PagedList<ProductAttribute>(productsAttributes, pageIndex, pageSize, productsAttributes.TotalCount);
        }
    }
}