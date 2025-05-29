using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Http.Extensions;
using Nop.Plugin.Widgets.CheckoutGiftMessage.Models;
using Nop.Services.Attributes;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;

namespace Nop.Plugin.Widgets.CheckoutGiftMessage.Controllers
{
    public class BambooCheckoutController : Nop.Web.Controllers.CheckoutController
    {
        private readonly ISettingService _settingService;
        private readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
        private readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;

        public BambooCheckoutController(AddressSettings addressSettings, CaptchaSettings captchaSettings, CustomerSettings customerSettings, IAddressModelFactory addressModelFactory, IAddressService addressService, IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser, ICheckoutModelFactory checkoutModelFactory, ICountryService countryService, ICustomerService customerService, IGenericAttributeService genericAttributeService, ILocalizationService localizationService, ILogger logger, IOrderProcessingService orderProcessingService, IOrderService orderService, IPaymentPluginManager paymentPluginManager, IPaymentService paymentService, IProductService productService, IShippingService shippingService, IShoppingCartService shoppingCartService, IStoreContext storeContext, ITaxService taxService, IWebHelper webHelper, IWorkContext workContext, OrderSettings orderSettings, PaymentSettings paymentSettings, RewardPointsSettings rewardPointsSettings, ShippingSettings shippingSettings, TaxSettings taxSettings,

            ISettingService settingService,
            IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
            IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService

            )

            : base(addressSettings, captchaSettings, customerSettings, addressModelFactory, addressService, addressAttributeParser, checkoutModelFactory, countryService, customerService, genericAttributeService, localizationService, logger, orderProcessingService, orderService, paymentPluginManager, paymentService, productService, shippingService, shoppingCartService, storeContext, taxService, webHelper, workContext, orderSettings, paymentSettings, rewardPointsSettings, shippingSettings, taxSettings)
        {
            _settingService = settingService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public override async Task<IActionResult> OpcSavePaymentInfo(IFormCollection form)
        {
            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                var customer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                var paymentMethod = await _paymentPluginManager
                                        .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id)
                                    ?? throw new Exception("Payment method is not selected");

                var warnings = await paymentMethod.ValidatePaymentFormAsync(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);
                    //set previous order GUID (if exists)
                    await _paymentService.GenerateOrderGuidAsync(paymentInfo);

                    //session save
                    await HttpContext.Session.SetAsync("OrderPaymentInfo", paymentInfo);

                    var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "gift-msg",
                            html = await RenderPartialViewToStringAsync("~/Plugins/Widgets.CheckoutGiftMessage/Views/Checkout/OpcGiftMessage.cshtml", new GiftMessageModel())
                        },
                        goto_section = "gift_msg"
                    });
                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        html = await RenderPartialViewToStringAsync("OpcPaymentInfo", paymenInfoModel)
                    }
                });
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OpcSaveGiftMessage(GiftMessageModel model, IFormCollection form)
        {
            //validation
            if (_orderSettings.CheckoutDisabled)
                throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                throw new Exception("Your cart is empty");

            if (!_orderSettings.OnePageCheckoutEnabled)
                throw new Exception("One page checkout is disabled");

            if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                throw new Exception("Anonymous checkout is not allowed");

            // Save gift message to checkout attributes
            if (string.IsNullOrEmpty(model.Message) == false)
            {
                int giftMessageCheckoutAttributeId = await _settingService.GetSettingByKeyAsync<int>(Defaults.GIFT_CARD_CHECKOUT_ATTRIBUTE_SETTING_KEY);
                CheckoutAttribute giftMessageCheckoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(giftMessageCheckoutAttributeId);
                string attributesXml = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, store.Id);
                attributesXml = _checkoutAttributeParser.AddAttribute(attributesXml, giftMessageCheckoutAttribute, model.Message.Trim());
                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.CheckoutAttributes, attributesXml, store.Id);
            }

            var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "confirm-order",
                    html = await RenderPartialViewToStringAsync("~/Views/Checkout/OpcConfirmOrder.cshtml", confirmOrderModel)
                },
                goto_section = "confirm_order"
            });
        }
    }
}