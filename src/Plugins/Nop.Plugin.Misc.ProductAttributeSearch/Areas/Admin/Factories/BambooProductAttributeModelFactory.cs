using Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Models;
using Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Services;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Factories
{
    public class BambooProductAttributeModelFactory : IBambooProductAttributeModelFactory
    {
        private readonly IBambooProductAttributeService _bambooProductAttributeService;

        public BambooProductAttributeModelFactory(IBambooProductAttributeService bambooProductAttributeService)
        {
            _bambooProductAttributeService = bambooProductAttributeService;
        }

        public async Task<ProductAttributeListModel> PrepareProductAttributeListModelAsync(ProductAttributeSearchModel searchModel)
        {
            ArgumentNullException.ThrowIfNull(searchModel);

            var productAttributes = await _bambooProductAttributeService.GetAllProductAttributesAsync(productAttributeName: searchModel.SearchProductAttributeName, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ProductAttributeListModel().PrepareToGrid(searchModel, productAttributes, () =>
            {
                //fill in model values from the entity
                return productAttributes.Select(attribute => new ProductAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name
                });
            });

            return model;
        }
    }
}