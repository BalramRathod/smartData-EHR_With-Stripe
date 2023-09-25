using EHR_project.stripeModel;
using EHR_project.StripeServices;
using Microsoft.AspNetCore.Mvc;

namespace EHR_project.Controllers
{
    public class StripeController : Controller
    {
        
        private readonly IStripeAppService _stripeService;

        public StripeController(IStripeAppService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost("payment/Singlepayment")]
        public async Task<ActionResult<StripePayment>> AddStripePayment(
            [FromBody] AddStripePayment payment,
            CancellationToken ct)
        {
            StripePayment createdPayment = await _stripeService.AddStripePaymentAsync(
                payment,
                ct);
            return Ok(new { success = true, message = "Payment Successfull.", createdPayment });
            // return StatusCode(StatusCodes.Status200OK, createdPayment);
        }

    }
}
