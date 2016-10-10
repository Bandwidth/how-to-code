using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;

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
      services.AddMvc();
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

      private const string BusinessNumber = "+1234567890"; // your real number

      private static readonly List<CallData> Calls = new List<CallData>(); // use DB instead of it in real apps

      // POST demo/callCallback
      [HttpPost]
      public async void CallCallback([FromQuery] CallbackEvent eventData, [FromQuery] string originalCallId)
      {
        var client = new Client(UserId, Token, Secret);
        if (string.IsNullOrEmpty(originalCallId))
        {
          switch (eventData.EventType)
          {
            case CallbackEventType.Answer:
              Calls.Add(new CallData
              {
                From = eventData.From,
                To = eventData.To,
                Forwarded = BusinessNumber,
                CallId = eventData.CallId,
                State = "ringing"
              });
              await
                client.Call.TransferAsync(eventData.CallId, BusinessNumber, null, null,
                  $"http://{Request.Host.ToUriComponent()}/demo/callCallback?originalCallId={eventData.CallId}");
              break;
            case CallbackEventType.Hangup:
              var call = Calls.FirstOrDefault(c => c.CallId == eventData.CallId);
              if (call?.State == "ringing")
              {
                call.State = "not answered";
              }
              break;
          }
        }
        else
        {
          var call = Calls.FirstOrDefault(c => c.CallId == originalCallId);
          if (call != null)
          {
            switch (eventData.EventType)
            {
              case CallbackEventType.Answer:
                call.StartTime = DateTime.Now;
                call.State = "answered";
                break;
              case CallbackEventType.Hangup:
                call.EndTime = DateTime.Now;
                call.Duration = call.EndTime - call.StartTime;
                call.State = "completed";
                break;
            }
          }
        }
      }

      private const string ApplicationId = "appId"; //application id (it is required only for GetPhoneNumberToTrack())
      public static async Task<string> GetPhoneNumberToTrack()
      {
        var client = new Client(UserId, Token, Secret);
        var number =
          (await client.AvailableNumber.SearchLocalAsync(new LocalNumberQuery {AreaCode = "919", Quantity = 1})).First().Number;
        await client.PhoneNumber.CreateAsync(new CreatePhoneNumberData
        {
          Number = number,
          ApplicationId = ApplicationId
        });
        return number;
      }

    }
  }

  public class CallData
  {
    public string From { get; set; }
    public string To { get; set; }
    public string CallId { get; set; }
    public string Forwarded { get; set; }
    public string State { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
  }
}

/*
 Before run this app do next:
 - Create an application on Catapult dashboard. Set incoming call url as http://<your-host>/demo/callCallback
 - Allocate new or use exiting phone number to track in Catapult and assign it to the application. Use this number as number for incoming calls from users. As alternative call CallCallback.GetPhoneNumberToTrack() to get new number to track
 */
