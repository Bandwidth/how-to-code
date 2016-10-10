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
      private const string ApplicationId = "a-your-app-id"; //{app_id}

      private string _number;

      private string _sipUri;

      // POST demo/sip
      [HttpPost]
      public async Task<string> Sip()
      {
        //Create SIP endpoint
        var client = new Client(UserId, Token, Secret);
        var domainId = await client.Domain.CreateAsync(new CreateDomainData
        {
          Name = "unique_domain_name", // SIP endpoint will be created in domain unique_domain_name.bwapp.bwsip.io
          Description = "description"
        });

        var endpoint = await client.Endpoint.CreateAsync(new CreateEndpointData
        {
          Name = "jsmith_mobile",
          Description = "John Smiths mobile client",
          DomainId = domainId,
          ApplicationId = ApplicationId,
          Credentials = new CreateEndpointCredentials {Password = "abc123"}
        });
        _sipUri = endpoint.Instance.SipUri;
        return _sipUri;
      }

      // POST demo/calltosip
      [HttpPost]
      public async void CallToSip()
      {
        var client = new Client(UserId, Token, Secret);
        await client.Call.CreateAsync(new CreateCallData
        {
          From = await GetNumber(client),
          To = _sipUri,
          CallbackUrl = $"http://{Request.Host.ToUriComponent()}/demo/calltosipcallback"
        });
      }

      // POST demo/calltosipcallback
      [HttpPost]
      public async void CallToSipCallback([FromBody] CallbackEvent eventData)
      {
        var client = new Client(UserId, Token, Secret);
        switch (eventData.EventType)
        {
          case CallbackEventType.Answer:
            await client.Call.SpeakSentenceAsync(eventData.CallId, "Hello sip client");
            break;
          case CallbackEventType.Speak:
            if (eventData.Status == CallbackEventStatus.Done)
            {
              // Greeting has been spoken. Complete the call
              await client.Call.HangupAsync(eventData.CallId);
            }
            break;
        }
      }

      // POST demo/callCallback
      [HttpPost]
      public async void CallCallback([FromBody] CallbackEvent eventData)
      {
        var client = new Client(UserId, Token, Secret);
        switch (eventData.EventType)
        {
          case CallbackEventType.Incomingcall:
            if (eventData.From == _sipUri)
            {
              await client.Call.CreateAsync(new CreateCallData
              {
                From = await GetNumber(client),
                To = eventData.To,
                Tag = eventData.CallId,
                CallbackUrl = $"http://{Request.Host.ToUriComponent()}/demo/anothercallcallback"
              });
              await client.Call.AnswerAsync(eventData.CallId);
              return;
            }
            var number = await GetNumber(client);
            if (eventData.To == number)
            {
              await client.Call.CreateAsync(new CreateCallData
              {
                From = number,
                To = _sipUri,
                Tag = eventData.CallId,
                CallbackUrl = $"http://{Request.Host.ToUriComponent()}/demo/anothercallcallback"
              });
              await client.Call.AnswerAsync(eventData.CallId);
            }
            break;
          case CallbackEventType.Answer:
            await client.Call.SpeakSentenceAsync(eventData.CallId, "Hello, please record your message at the beep.");
            break;
        }
      }

      // POST demo/anothercallcallback
      [HttpPost]
      public async void AnotherCallCallback([FromBody] CallbackEvent eventData)
      {
        var client = new Client(UserId, Token, Secret);
        switch (eventData.EventType)
        {
          case CallbackEventType.Answer:
            // Bridge calls
            await client.Bridge.CreateAsync(new CreateBridgeData
            {
              CallIds = new[] {eventData.CallId, eventData.Tag}
            });
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
          Number = number
        });
        _number = number;
        return _number;
      }
    }
  }
}

/*
 Before run this app do next:
 - Create an application on Catapult dashboard. Set incoming call url as http://<your-host>/demo/callCallback. Copy application Id and assign it to constant ApplicationId in this code
 Run app and then
 - Make POST request (with any data) to demo/sip to create sip endpoint (you will see sip uri in result)
 - Configure SIP client with shown sip uri and password abc123.
 - Make POST request (with any data) to demo/calltosip to make direct call to SIP client
 - Now you can test incoming/outgoung calls to/from SIP client
   */
