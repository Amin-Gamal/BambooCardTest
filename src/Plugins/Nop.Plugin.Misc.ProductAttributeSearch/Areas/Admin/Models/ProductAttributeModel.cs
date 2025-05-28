using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.ProductAttributeSearch.Areas.Admin.Models
{
    public record ProductAttributeModel : BaseNopEntityModel
    {
        public string Name { get; set; }
    }
}