//Download the .net sdk from ap.bandwidth.com/docs/helper-libraries/net

using System;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Api;

public class Program
{
  //API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
  private const string UserId = "u-YOUR_USER_ID"; //{user_id}
  private const string Token = "t-YOUR_API_TOKEN"; //{token}
  private const string Secret = "YOUR_API_SECRET"; //{secret}

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

    var call = await client.Call.CreateAsync(new CreateCallData
    {
      From = "+12525089000", // <-- This must be a Bandwidth number on your account
      To = "+15035555555",
      CallbackUrl = "http://requestb.in/10sze251"
    });
    Console.WriteLine($"Call Id is {call.Id}");
  }
}