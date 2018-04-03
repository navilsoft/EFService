using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace EFService
{
    class SMSTwilio
    {
        public string SendSmsToCustomer()
        {
            // Your Account SID from twilio.com/console
            var accountSid = "AC2e9a5b0a9c82c18df4e3e12621155950";
            // Your Auth Token from twilio.com/console
            var authToken = "auth_token";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                //to: new PhoneNumber("+15558675309"),
                //from: new PhoneNumber("+15017250604"),
                to: new PhoneNumber("+6583151070"),
                from: new PhoneNumber("+6583151070"),
                body: "Hello from C#");

            return message.Sid;
          
        }
    }
}
