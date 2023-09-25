using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace EHR_project.encryption
{
    public class SendsmsService
    {

        public string? phone { get; set; }
        public int? Otp { get; set; }
      

        public SendsmsService(string? Phone, int? otp )
        {
            phone = Phone;

            Otp = otp;
           
        }
        public void sendSms()
        {
            try
            {
                var accountSid = "ACbe697aa57c59e6a39ece6629ded85b10";
                var authToken = "43e7d5a261a9a1f25192e5b3d4646923";
                TwilioClient.Init(accountSid, authToken);

                var messageOptions = new CreateMessageOptions(
                    new PhoneNumber("+91" + phone));
                
                messageOptions.From = new PhoneNumber("+12179926780");
                messageOptions.Body = "registration completed successfully!" + "OTP"
                    + Otp;

                var message = MessageResource.Create(messageOptions);
                Console.WriteLine(message.Body);


            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }
}

