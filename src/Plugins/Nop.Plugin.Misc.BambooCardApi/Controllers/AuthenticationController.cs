using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.BambooCardApi.Models;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace Nop.Plugin.Misc.BambooCardApi.Controllers
{
    //[Route("api")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;

        public AuthenticationController(IAuthenticationService authenticationService,
                                        ICustomerRegistrationService customerRegistrationService,
                                        ICustomerService customerService)
        {
            _authenticationService = authenticationService;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
        }

        [HttpPost]
        [Route("api/authenticate", Name = "authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticationRequestBody authenticationRequestBody)
        {
            var loginResult = await _customerRegistrationService.ValidateCustomerAsync(authenticationRequestBody.Email, authenticationRequestBody.Password);

            if (loginResult != CustomerLoginResults.Successful)
                return StatusCode((int)HttpStatusCode.Unauthorized, "Wrong username or password");

            var customer = await _customerService.GetCustomerByEmailAsync(authenticationRequestBody.Email);

            if (customer == null)
                return StatusCode((int)HttpStatusCode.Unauthorized, "Wrong username or password");

            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(Defaults.SECURITY_KEY));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var currentTime = DateTimeOffset.Now;
            var claimsForToken = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Nbf, currentTime.ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, currentTime.AddDays(30).ToUnixTimeSeconds().ToString()),

                new Claim(ClaimTypes.NameIdentifier, customer.CustomerGuid.ToString()),
                new Claim(ClaimTypes.Email, customer.Email)
            };

            var jwtSecurityToken = new JwtSecurityToken(new JwtHeader(signingCredentials), new JwtPayload(claimsForToken));
            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            await _authenticationService.SignInAsync(customer, authenticationRequestBody.RememberMe);

            return Ok(tokenToReturn);
        }
    }
}