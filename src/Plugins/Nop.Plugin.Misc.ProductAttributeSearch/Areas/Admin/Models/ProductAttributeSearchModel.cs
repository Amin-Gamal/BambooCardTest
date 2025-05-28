using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Models
{
    public record ProductAttributeSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Plugins.Misc.ProductAttributeSearch.Fields.SearchProductAttributeName")]
        public string SearchProductAttributeName { get; set; }
    }
}