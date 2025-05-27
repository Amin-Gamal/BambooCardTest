namespace Nop.Plugin.DiscountRules.BambooCardDiscounts.Services
{
    public interface IBambooCardDiscountsService
    {
        Task<int> GetNumberOfCompletedOrdersByCustomerIdAsync(int customerId);
    }
}