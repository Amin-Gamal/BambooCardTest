using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.BambooCardApi.RequestFeatures;

namespace Nop.Plugin.Misc.BambooCardApi.Services
{
    public interface IBambooCardOrderService
    {
        Task<PagedList<Order>> GetOrderDetailsById(int customerId, int pageIndex, int pageSize);
    }
}