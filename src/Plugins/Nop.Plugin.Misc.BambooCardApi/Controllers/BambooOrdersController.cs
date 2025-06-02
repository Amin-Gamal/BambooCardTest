using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.BambooCardApi.DTO;
using Nop.Plugin.Misc.BambooCardApi.RequestFeatures;
using Nop.Plugin.Misc.BambooCardApi.Services;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using System.Text.Json;

namespace Nop.Plugin.Misc.BambooCardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BambooOrdersController : ControllerBase
    {
        private readonly IBambooCardOrderService _bambooCardOrderService;
        private readonly ICustomerService _customerService;
        private readonly IAuthenticationService _authenticationService;

        public BambooOrdersController(IBambooCardOrderService bambooCardOrderService,
                                      ICustomerService customerService,
                                      IAuthenticationService authenticationService)
        {
            _bambooCardOrderService = bambooCardOrderService;
            _customerService = customerService;
            _authenticationService = authenticationService;
        }

        [HttpGet("customerOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<OrderDetailsDto>>> GetOrdersDetailsByEmail([FromQuery] OrderDetailsParameters orderDetailsParameters)
        {
            if (string.IsNullOrEmpty(orderDetailsParameters.Email))
                return BadRequest("Email cannot be null or empty.");

            if (await CheckPermissions(orderDetailsParameters.Email) == false)
                return new StatusCodeResult(StatusCodes.Status403Forbidden);

            Customer customer = await _customerService.GetCustomerByEmailAsync(orderDetailsParameters.Email);

            if (customer == null)
                return NotFound("Customer not found.");

            var orderDetails = await _bambooCardOrderService.GetOrderDetailsById(customer.Id, orderDetailsParameters.PageIndex, orderDetailsParameters.PageSize);

            var result = orderDetails

                .Select(o =>
                {
                    return new OrderDetailsDto
                    {
                        Id = o.Id,
                        TotalAmount = o.OrderTotal,
                        OrderDate = DateOnly.FromDateTime(o.CreatedOnUtc)
                    };
                });

            var metaData = new
            {
                orderDetails.TotalCount,
                orderDetails.PageSize,
                orderDetails.CurrentPage,
                orderDetails.TotalPages,
                orderDetails.HasNext,
                orderDetails.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metaData));

            return Ok(result);
        }

        private async Task<bool> CheckPermissions(string customerEmail)
        {
            var currentCustomer = await _authenticationService.GetAuthenticatedCustomerAsync();

            if (currentCustomer is null)
                return false;

            if (currentCustomer.Email == customerEmail)
                return true; // allow access to own orders

            if (await _customerService.IsAdminAsync(currentCustomer))
                return true; // if customer is admin, allow access to all orders

            return false; // otherwise, deny access to other customers' orders
        }
    }
}