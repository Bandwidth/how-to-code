//Download the .net sdk from ap.bandwidth.com/docs/helper-libraries/net
using System;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Api;

public class Program
{
  //API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
  private const string UserId = "u-userID";  //{user_id}
  private const string Token = "t-token"; //{token}
  private const string Secret = "secret"; //{secret}

  public static void Main()
  {
    try
    {
      RunAsync().Wait();
    }
    catch (Exception ex)
    {
      Console.Error.WriteLine(ex.Message);
      Environment.ExitCode = 1;
    }
  }

  private static async Task RunAsync()
  {
    var client = new Client(UserId, Token, Secret);
    var app = await client.Application.CreateAsync(new CreateApplicationData
    {
      Name = "SampleApp2",
      IncomingCallUrl = "http://your-server.com/CallCallback",
      IncomingMessageUrl = "http://your-server.com/MsgCallback"
    });
    Console.WriteLine($"Created application with id {app.Id}");
  }
}
