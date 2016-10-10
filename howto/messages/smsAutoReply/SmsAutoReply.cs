using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Api;
using Bandwidth.Net.Xml;
using Bandwidth.Net.Xml.Verbs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
      private const string PhoneNumber = "+1234567891"; // your exisitng phone number in Bandwidth


      // GET demo/messageCallback
      [HttpGet]
      public IActionResult MessageCallback([FromQuery] CallbackEvent eventData)
      {
        switch (eventData.EventType)
        {
          case CallbackEventType.Sms:

            var bxmlResponse = new Response(new SendMessage
            {
              From = PhoneNumber,
              To = eventData.From,
              Text = "AutoReply Message"
            });
            return new ContentResult {Content = bxmlResponse.ToXml(), ContentType = "text/xml"};
        }
        return new NoContentResult();
      }
    }
  }
}

/*
 Before run this app do next:
 - Create an application on Catapult dashboard. Set incoming message url as http://<your-host>/demo/messageCallback and method GET.
 - Allocate new or use exiting phone number in Catapult. Copy it into constant `PhoneNumber`
 */