//NOTE: After running this app make POST request to demo/remind (with any data) to make this app to call you with remind

using System.Linq;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

// You can run this app via IIS or via self-hosted app
namespace WebApplication
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var host = new WebHostBuilder()
        .UseKestrel()
        .UseIISIntegration()
        .UseStartup<Startup>()
        .Build();

      host.Run();
    }
  }

  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services
        .AddMvc()
        .AddJsonOptions(s => s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      app.UseMvcWithDefaultRoute();
    }
  }

  namespace Controllers
  {
    public class DemoController : Controller
    {
      //API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
      private const string UserId = "u-userID"; //{user_id}
      private const string Token = "t-token"; //{token}
      private const string Secret = "secret"; //{secret}

      private const string PhoneNumber = "+1-your-real-number"; //real phone number to receive incoming call

      private static string _number;


      // POST demo/remind
      [HttpPost]
      public async void Remind()
      {
        // Call to {PhoneNumber} with reminder
        var client = new Client(UserId, Token, Secret);
        var number = await GetNumber(client);
        await client.Call.CreateAsync(new CreateCallData
        {
          From = number,
          To = PhoneNumber,
          CallbackUrl = $"http://{Request.Host.ToUriComponent()}/demo/callCallback"
        });
      }

      // POST demo/callCallback
      [HttpPost]
      public async void CallCallback([FromBody] CallbackEvent eventData)
      {
        var client = new Client(UserId, Token, Secret);
        var number = await GetNumber(client);
        if (number != eventData.To) return;
        switch (eventData.EventType)
        {
          case CallbackEventType.Answer:
            await client.Call.CreateGatherAsync(eventData.CallId, new CreateGatherData
            {
              MaxDigits = "1",
              InterDigitTimeout = "20",
              Prompt = new GatherPrompt
              {
                Sentence =
                  "Hello David, you have an appointment on tuesday.  Please press 1 to confirm and press 2 to reschedule.",
                Gender = Gender.Female,
                Locale = "en",
                Bargeable = true
              }
            });
            break;
          case CallbackEventType.Gather:
            if (eventData.State == CallbackEventState.Completed && eventData.Reason == CallbackEventReason.MaxDigits)
            {
              // Do something with pressed digit eventData.Digits
              await client.Call.SpeakSentenceAsync(eventData.CallId, "Thank you");
            }
            break;
          case CallbackEventType.Speak:
            if (eventData.Status == CallbackEventStatus.Done)
            {
              // `Thank you` has been spoken. Complete the call
              await client.Call.HangupAsync(eventData.CallId);
            }
            break;
        }
      }

      private async Task<string> GetNumber(Client client)
      {
        if (_number != null)
        {
          return _number;
        }
        // Allocate a number to make outgoing call
        var number = (await client.AvailableNumber.SearchLocalAsync(new LocalNumberQuery
        {
          AreaCode = "919",
          Quantity = 1
        })).Select(n => n.Number).First();
        await client.PhoneNumber.CreateAsync(new CreatePhoneNumberData
        {
          Number = number,
        });
        _number = number;
        return _number;
      }
    }
  }
}