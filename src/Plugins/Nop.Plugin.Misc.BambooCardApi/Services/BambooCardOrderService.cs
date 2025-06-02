using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.BambooCardApi.RequestFeatures;

namespace Nop.Plugin.Misc.BambooCardApi.Services
{
    public class BambooCardOrderService : IBambooCardOrderService
    {
        private readonly IRepository<Order> _orderRepository;

        public BambooCardOrderService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PagedList<Order>> GetOrderDetailsById(int customerId, int pageIndex, int pageSize)
        {
            var orders = _orderRepository.Table.Where(o => o.CustomerId == customerId && o.Deleted == false);

            return PagedList<Order>.ToPagedList(orders.OrderBy(o => o.CreatedOnUtc), pageIndex, pageSize);
        }
    }
}