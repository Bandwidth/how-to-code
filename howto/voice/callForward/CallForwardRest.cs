using System.Linq;
using System.IO;
using Bandwidth.Net;
using Bandwidth.Net.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

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
      // API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
      private const string UserId = "u-userID"; // {user_id}
      private const string Token = "t-token"; // {token}
      private const string Secret = "secret"; // {secret}

      private const string ForwardToPhoneNumber = "+1xxxxx"; // phone number to forward call
      private const string ApplicationId = "a-xxxxx"; // created application Id on Catapult dashboard

      // POST demo/forward
      [HttpPost]
      public async string Forward()
      {
        string json;
        var client = new Client(UserId, Token, Secret);
        using (var reader = new StreamReader(Request.Body))
        {
          json = await reader.ReadToEndAsync();
        }
        var eventData = CallbackEvent.CreateFromJson(json);
        switch (eventData.EventType)
        {
          case CallbackEventType.Incomingcall:
            var app = await client.Application.GetAsync(ApplicationId);
            if (!app.AutoAnswer)
            {
              // Answer call itself if AutoAnswer = false
              // You can remove this handler of CallbackEventType.Incomingcall if you configure AutoAnswer = true for the Catapult app on dashboard
              await client.Call.AnswerAsync(eventData.CallId);
            }
            break;
          case CallbackEventType.Answer:
            await client.Call.TransferAsync(eventData.CallId, ForwardToPhoneNumber);
            break;
        }
        return "";
      }
    }
  }
}

/*
 Before run this app do next:
 - Create an application on Catapult dashboard. Set incoming call url as http://<your-host>/demo/forward and method to POST (default)
 - Allocate new or use exiting phone number in Catapult and assign it to the application.
 */
