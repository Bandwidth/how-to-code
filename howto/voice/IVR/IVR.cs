using System.Threading.Tasks;
using Bandwidth.Net;
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
      //API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
      private const string UserId = "u-userID"; //{user_id}
      private const string Token = "t-token"; //{token}
      private const string Secret = "secret"; //{secret}

      private const string AgentPhoneNumber = "+1234567890"; // use real phone number here

      // GET demo/callCallback
      [HttpGet]
      public IActionResult CallCallback([FromQuery] CallbackEvent eventData)
      {
        switch (eventData.EventType)
        {
          case CallbackEventType.Incomingcall:

            var bxmlResponse = new Response(
              new Call
              {
                To = AgentPhoneNumber,
                From = eventData.To,
                RequestUrl = $"/demo/outboundCall?inboundCallId={eventData.CallId}&conferenceNumber={eventData.To}"
              },
              new SpeakSentence { Sentence = "Please wait while we transfer you to your party", Voice = "julie" },
              new Conference { From = eventData.To}
            );
            return new ContentResult { Content = bxmlResponse.ToXml(), ContentType = "text/xml" };
        }
        return new NoContentResult();
      }

      // GET demo/outboundCall
      [HttpGet]
      public IActionResult OutboundCall([FromQuery] CallbackEvent eventData, [FromQuery]string inboundCallId, [FromQuery]string conferenceNumber)
      {
        switch (eventData.EventType)
        {
          case CallbackEventType.Answer:

            var bxmlResponse = new Response(
              new Gather
              {
                MaxDigits = 1,
                RequestUrl = $"/demo/gatherCallback?inboundCallId={inboundCallId}&conferenceNumber={conferenceNumber}",
                SpeakSentence = new SpeakSentence
                {
                  Sentence = $"You have a call from {eventData.From}. Press one to answer the call or two to send to voice mail.",
                  Voice = "julie"
                }
              }
            );
            return new ContentResult { Content = bxmlResponse.ToXml(), ContentType = "text/xml" };
        }
        return new NoContentResult();
      }

      // GET demo/gatherCallback
      [HttpGet]
      public IActionResult GatherCallback([FromQuery] string digits, [FromQuery] string inboundCallId, [FromQuery] string conferenceNumber)
      {
        var bxmlResponse = new Response();
        switch (digits)
        {
          case "1":
            // join to the conference
            bxmlResponse.Add(new SpeakSentence { Sentence = "Connecting to your call", Voice = "julie" });
            bxmlResponse.Add(new Conference { From = conferenceNumber });
            break;
          case "2":
            // redirect to voice mail
            bxmlResponse.Add(new Redirect {Context = inboundCallId, RequestUrl = "/demo/voicemailCallback"});
            break;
          default:
            return new NoContentResult();
        }
        return new ContentResult { Content = bxmlResponse.ToXml(), ContentType = "text/xml" };
      }

      // GET demo/voicemailCallback
      [HttpGet]
      public IActionResult VoiceMailCallback()
      {
        var bxmlResponse = new Response(
          new SpeakSentence
          {
            Sentence = "We're sorry. Your party is not available to take your call. Please leave a message at the beep. Press eight when you are done.",
            Voice = "julie"
          },
          new Record
          {
            TerminatingDigits = "8",
            RequestUrl = "/demo/recordCallback"
          }
        );
        return new ContentResult { Content = bxmlResponse.ToXml(), ContentType = "text/xml" };
      }

      // GET demo/recordCallback
      [HttpGet]
      public async Task<IActionResult> RecordCallback([FromQuery] string recordingId)
      {
        var client = new Client(UserId, Token, Secret);
        var recording = await client.Recording.GetAsync(recordingId);
        // Do something with URL of voice data in recording.Media.
        // For example play it
        var bxmlResponse = new Response(new PlayAudio {Url = recording.Media});
        return new ContentResult { Content = bxmlResponse.ToXml(), ContentType = "text/xml" };
      }

    }
  }
}

/*
 Before run this app do next:
 - Create an application on Catapult dashboard. Set incoming call url as http://<your-host>/demo/callCallback and method to GET
 - Allocate new or use exiting phone number in Catapult and assign it to the application. Use this number as number for incoming calls from users.
 */
