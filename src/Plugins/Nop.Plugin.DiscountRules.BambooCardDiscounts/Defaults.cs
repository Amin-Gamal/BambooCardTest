namespace Nop.Plugin.DiscountRules.BambooCardDiscounts
{
    public static class Defaults
    {
        public const string BAMBOO_CARD_DISCOUNTS_NAME = "BambooCard Discount";

        public const string BAMBOO_CARD_DISCOUNTS_SETTING_KEY = "BambooCard-Discount";

        public const string BAMBOO_CARD_DISCOUNTS_SETTING_VALUE = "BambooCard-Discount-{0}";

        public const string BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SYSTEM_NAME = "DiscountRequirement.MustHaveNumberOfOrders";

        public const string BAMBOO_CARD_DISCOUNTS_REQUIREMENT_SETTING_KEY = "DiscountRequirement.MustHaveNumberOfOrders-{0}";

        public const int MINIMUM_NUMBER_OF_ORDERS_DEFAULT_VALUE = 3;

        public const string HTML_FIELD_PREFIX = "DiscountRulesBambooCardDiscounts{0}";
    }
}