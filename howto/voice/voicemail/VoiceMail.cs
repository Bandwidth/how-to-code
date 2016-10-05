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

      // POST demo/callCallback
      [HttpPost]
      public async void CallCallback([FromBody] CallbackEvent eventData)
      {
        var client = new Client(UserId, Token, Secret);
        switch (eventData.EventType)
        {
          case CallbackEventType.Incomingcall:
            // Do nothing because AutoAnswer is true for the application (you should configure it on Catapult dashboard)
            // otherwite you should call
            // await client.Call.AnswerAsync(eventData.CallId);
            break;
          case CallbackEventType.Answer:
            await client.Call.SpeakSentenceAsync(eventData.CallId, "Hello, please record your message at the beep.");
            break;
          case CallbackEventType.Speak:
            if (eventData.Status == CallbackEventStatus.Done)
            {
              // Greeting has been spoken. Play 'beep'
              await client.Call.PlayAudioFileAsync(eventData.CallId, "https://s3.amazonaws.com/bwdemos/beep.mp3");
            }
            break;
          case CallbackEventType.Playback:
            if (eventData.Status == CallbackEventStatus.Done)
            {
              // 'Beep' has been played. Start recording
              await client.Call.TurnCallRecordingAsync(eventData.CallId, true);
            }
            break;
          case CallbackEventType.Recording:
            if (eventData.State == CallbackEventState.Complete)
            {
              // Get recording file url
              var recording = await client.Recording.GetAsync(eventData.RecordingId);

              // TODO Save recording.Media or recording.MediaName to database to use it in future.
              // Sample code to download recording file
              // var content = client.Media.DownloadAsync(recording.MediaName);
            }
            break;
        }
      }
    }
  }
}