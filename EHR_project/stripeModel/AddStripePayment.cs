namespace EHR_project.stripeModel
{
    public record AddStripePayment(
      // string CustomerId,
      string ReceiptEmail,
       // string Description,
       string Currency,
       string source,
       long Amount);
}
