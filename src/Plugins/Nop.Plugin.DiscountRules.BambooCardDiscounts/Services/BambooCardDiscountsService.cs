using Nop.Core.Domain.Orders;
using Nop.Data;

namespace Nop.Plugin.DiscountRules.BambooCardDiscounts.Services
{
    public class BambooCardDiscountsService : IBambooCardDiscountsService
    {
        private readonly INopDataProvider _nopDataProvider;

        public BambooCardDiscountsService(INopDataProvider nopDataProvider)
        {
            _nopDataProvider = nopDataProvider;
        }

        public async Task<int> GetNumberOfCompletedOrdersByCustomerIdAsync(int customerId)
        {
            if (customerId == 0)
                return 0;

            string sql = "SELECT COUNT(Id) FROM [Order] WHERE CustomerId = @CustomerId AND OrderStatusId = @OrderStatus";

            var parameters = new LinqToDB.Data.DataParameter[]
            {
                new LinqToDB.Data.DataParameter
                {
                    Name = "CustomerId",
                    Value = customerId,
                    DataType = LinqToDB.DataType.Int32,
                },
                new LinqToDB.Data.DataParameter
                {
                    Name = "OrderStatus",
                    Value = (int)OrderStatus.Complete,
                    DataType = LinqToDB.DataType.Int32,
                }
            };

            var result = await _nopDataProvider.QueryAsync<int>(sql, parameters);

            return result.FirstOrDefault();
        }
    }
}