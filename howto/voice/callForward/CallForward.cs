using System.Linq;
using Bandwidth.Net;
using Bandwidth.Net.Xml;
using Bandwidth.Net.Xml.Verbs;
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

      // GET demo/forward
      [HttpGet]
      public IActionResult Forward([FromQuery] CallbackEvent eventData, [FromQuery] string phoneNumber)
      {
        if(!string.IsNullOrEmpty(phoneNumber))
        {
          switch (eventData.EventType)
          {
            case CallbackEventType.Answer:

              var bxmlResponse = new Response(new Transfer
              {
                TransferCallerId = eventData.From,
                PhoneNumbers = new[] {phoneNumber}
              });
              return new ContentResult {Content = bxmlResponse.ToXml(), ContentType = "text/xml"};
          }
        }
        return new NoContentResult();
      }
    }
  }
}

/*
 Before run this app do next:
 - Create an application on Catapult dashboard. Set incoming call url as http://<your-host>/demo/forward=YOUR-PHONE-NUMBER-TO-FORWARD and method to GET
 - Allocate new or use exiting phone number in Catapult and assign it to the application.
 */
