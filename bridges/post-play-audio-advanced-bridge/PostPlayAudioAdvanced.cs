//Download the .net sdk from ap.bandwidth.com/docs/helper-libraries/net
using System;
using System.Threading.Tasks;
using Bandwidth.Net;

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

    //Play Audio File on loop
    await client.Bridge.PlayAudioAsync("bridgeId", new PlayAudioData
    {
      FileUrl = "http://myurl.com/file.mp3",
      LoopEnabled = true
    });

    //Speak sentence with options
    await client.Bridge.PlayAudioAsync("bridgeId", new PlayAudioData
    {
      Sentence = "hola de Bandwidth",
      Gender = Gender.Male,
      Locale = "es",
      Voice  = "Jorge"
    });
  }
}
