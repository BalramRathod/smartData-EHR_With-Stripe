using EHR_project.Data;
using EHR_project.Dto;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace EHR_project.Controllers
{
    
        [Route("api/[controller]")]
        [ApiController]
        public class PaymentsController : ControllerBase
        {
            private readonly DBContext _context;

            private DapperContext _Connectionstring;

            public PaymentsController(DBContext context)
            {
                _context = context;
            }

            [HttpPost("StripePayment")]
            public async Task<ActionResult> StripePayment(StripePaymentDto data)
            {
                try
                {
                    StripeConfiguration.ApiKey = "sk_test_51NVXNqBJS0gzkSXcIEprJlHqQLtmtYBKaLdTZlzHhE1cmMSRs1ANxaXm7dpFWg50w8aO4XRB18sf5SppQbSjwNQ600Ad8ped52";

                    // `source` is obtained with Stripe.js; see https://stripe.com/docs/payments/accept-a-payment-charges#web-create-token
                    var options = new ChargeCreateOptions
                    {
                        Amount = data.Amount*100,
                        Currency = "usd",   
                        Source = data.Token,
                        Description = "Direct Payment Without Customer",
                    };
                    var service = new ChargeService();
                    var res = service.Create(options);

                    return Ok(new
                    {
                        message = true,
                        chargeId = res.Id
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new { message = ex.Message });
                }
            }

            [HttpPost("StripePaymentRefund/{id}")]
            public async Task<ActionResult> StripePaymentRefund(string id)
            {
                try
                {
                    StripeConfiguration.ApiKey = "sk_test_51NVXNqBJS0gzkSXcIEprJlHqQLtmtYBKaLdTZlzHhE1cmMSRs1ANxaXm7dpFWg50w8aO4XRB18sf5SppQbSjwNQ600Ad8ped52";

                    var options = new RefundCreateOptions
                    {
                        Charge = id,
                    };
                    var service = new RefundService();
                    var res = service.Create(options);
                    return Ok(new
                    {
                        message = true,
                        refundId = res.Id
                    });
                }
                catch (Exception ex)
                {
                    return Ok(new { message = ex.Message });
                }
            }
        }
    }

