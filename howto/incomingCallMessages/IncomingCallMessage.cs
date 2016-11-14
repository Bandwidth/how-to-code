//Download the .net sdk from ap.bandwidth.com/docs/helper-libraries/net

using System;
using System.Threading.Tasks;
using Bandwidth.Net;
using Bandwidth.Net.Api;

public class Program
{
  //API credentials which can be found on your account page at https://catapult.inetwork.com/pages/login.jsf
  private const string UserId = "{{userId}}"; //{user_id}
  private const string Token = "{{apiToken}}"; //{token}
  private const string Secret = "{{apiSecret}}"; //{secret}

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
      Name = "MyFirstApp",
      IncomingCallUrl = "http://example.com/calls.php",
      IncomingMessageUrl = "http://example.com/messages.php",
      CallbackHttpMethod = CallbackHttpMethod.Post,
      AutoAnswer = true
    });

    await client.PhoneNumber.UpdateAsync("{numberId}", new UpdatePhoneNumberData {ApplicationId = app.Id});
  }
}