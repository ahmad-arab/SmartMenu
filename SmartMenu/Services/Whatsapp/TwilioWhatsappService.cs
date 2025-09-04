using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;

namespace SmartMenu.Services.Whatsapp
{
    public class TwilioWhatsappService : IWhatsappService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;
        private readonly string _contentTemplateSid;

        public TwilioWhatsappService(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _fromNumber = configuration["Twilio:WhatsappFrom"];
            _contentTemplateSid = configuration["Twilio:ContentTemplateSID"];
        }

        public async Task SendMessageAsync(string to, string message)
        {
            TwilioClient.Init(_accountSid, _authToken);
            
            var toWhatsApp = new PhoneNumber($"whatsapp:{to}");
            var fromWhatsApp = new PhoneNumber($"whatsapp:{_fromNumber}");
            
            var messageOptions = new CreateMessageOptions(toWhatsApp);
            messageOptions.From = fromWhatsApp;
            messageOptions.Body = message;

            await MessageResource.CreateAsync(messageOptions);
        }

        public async Task SendTemplateMessageAsync(string to, string menuTitle)
        {
            TwilioClient.Init(_accountSid, _authToken);

            var toWhatsApp = new PhoneNumber($"whatsapp:{to}");
            var fromWhatsApp = new PhoneNumber($"whatsapp:{_fromNumber}");

            var messageOptions = new CreateMessageOptions(toWhatsApp)
            {
                From = fromWhatsApp,
                ContentSid = _contentTemplateSid
            };

            Dictionary<string, string>? templateParameters = new Dictionary<string, string>
            {
                { "business", menuTitle }
            };
            // Twilio expects ContentVariables as a JSON string
            messageOptions.ContentVariables = System.Text.Json.JsonSerializer.Serialize(templateParameters);

            await MessageResource.CreateAsync(messageOptions);
        }
    }
}