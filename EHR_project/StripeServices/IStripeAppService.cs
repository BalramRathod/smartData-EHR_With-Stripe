using EHR_project.stripeModel;

namespace EHR_project.StripeServices
{
    public interface IStripeAppService
    {


        Task<StripePayment> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct);


    }
}
