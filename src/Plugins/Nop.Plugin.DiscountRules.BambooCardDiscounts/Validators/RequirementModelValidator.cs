using FluentValidation;
using Nop.Plugin.DiscountRules.BambooCardDiscounts.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts.Validators
{
    internal class RequirementModelValidator : BaseNopValidator<RequirementModel>
    {
        public RequirementModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.DiscountId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.DiscountId.Required"));
            RuleFor(model => model.MinimumNumberOfOrders)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.CustomerRoles.Fields.CustomerRoleId.Required"));
        }
    }
}