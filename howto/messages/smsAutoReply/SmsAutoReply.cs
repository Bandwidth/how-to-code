using System.IO;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Api;
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
            // API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
            private const string UserId = "u-userID"; // {user_id}
            private const string Token = "t-token"; // {token}
            private const string Secret = "secret"; // {secret}

            private const string PhoneNumber = "+1234567891"; // your exisitng phone number in Bandwidth


            // POST demo/messageCallback
            [HttpPost]
            public async Task<string> MessageCallback()
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
                    case CallbackEventType.Sms:

                        await client.Message.SendAsync(new MessageData
                        {
                            From = PhoneNumber,
                            To = eventData.From,
                            Text = "AutoReply Message"
                        });
                        break;
                }
                return "";
            }
        }
    }
}

/*
 Before run this app do next:
 - Create an application on Catapult dashboard. Set incoming message url as http://<your-host>/demo/messageCallback and method POST.
 - Allocate new or use exiting phone number in Catapult. Copy it into constant `PhoneNumber`. Fill constants `UserId`, `Token`, `Secret` with your auth data
 */
