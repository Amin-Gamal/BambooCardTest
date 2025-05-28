using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Factories;
using Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Models;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;

using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Controllers
{
    public class BambooProductAttributeController : Nop.Web.Areas.Admin.Controllers.ProductAttributeController
    {
        private readonly IBambooProductAttributeModelFactory _bambooProductAttributeModelFactory;

        public BambooProductAttributeController(ICustomerActivityService customerActivityService, ILocalizationService localizationService, ILocalizedEntityService localizedEntityService, INotificationService notificationService, IPermissionService permissionService, IProductAttributeModelFactory productAttributeModelFactory, IProductAttributeService productAttributeService,

            IBambooProductAttributeModelFactory bambooProductAttributeModelFactory

            ) : base(customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, productAttributeModelFactory, productAttributeService)
        {
            _bambooProductAttributeModelFactory = bambooProductAttributeModelFactory;
        }

        public override IActionResult Index()
        {
            return base.Index();
        }

        [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW)]
        public override async Task<IActionResult> List()
        {
            ProductAttributeSearchModel searchModel = new();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return View("~/Plugins/Misc.ProductAttributeSearch/Areas/Admin/Views/BambooProductAttribute/List.cshtml", searchModel);
        }

        [HttpPost]
        [CheckPermission(StandardPermission.Catalog.PRODUCT_ATTRIBUTES_VIEW)]
        public virtual async Task<IActionResult> BambooProductAttributeList(ProductAttributeSearchModel searchModel)
        {
            //prepare model
            var model = await _bambooProductAttributeModelFactory.PrepareProductAttributeListModelAsync(searchModel);

            return Json(model);
        }
    }
}