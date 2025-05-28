using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Controllers
{
    public class BambooProductAttributeController : Nop.Web.Areas.Admin.Controllers.ProductAttributeController
    {
        public BambooProductAttributeController(ICustomerActivityService customerActivityService, ILocalizationService localizationService, ILocalizedEntityService localizedEntityService, INotificationService notificationService, IPermissionService permissionService, IProductAttributeModelFactory productAttributeModelFactory, IProductAttributeService productAttributeService) : base(customerActivityService, localizationService, localizedEntityService, notificationService, permissionService, productAttributeModelFactory, productAttributeService)
        {
        }


        public override IActionResult Index()
        {
            return base.Index();
        }

        public override Task<IActionResult> List()
        {
            return base.List();
        }

        public override Task<IActionResult> List(ProductAttributeSearchModel searchModel)
        {
            return base.List(searchModel);
        }

       
    }
}
