using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts.Models
{
    public class RequirementModel
    {
        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

        [NopResourceDisplayName("Plugins.DiscountRules.BambooCardDiscounts.Fields.MinimumNumberOfOrders")]
        public int MinimumNumberOfOrders { get; set; }
    }
}