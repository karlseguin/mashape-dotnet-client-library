using System;

namespace Mashape
{
   public interface IConfiguration
   {
      IConfiguration NetworkAvailableCheck(Func<bool> networkCheck);
      IConfiguration ConnectTo(string url);
   }

   public class Driver : IConfiguration
   {
      public static readonly Driver Instance = new Driver();

      public string PublicKey { get; private set; }
      public string PrivateKey { get; private set; }
      public Func<bool> NetworkCheck { get; private set; }
      public string Url { get; private set; }

      public static void Configure(string publicKey, string privateKey)
      {
         Instance.PublicKey = publicKey;
         Instance.PrivateKey = privateKey;
      }

      public static void Configure(Action<IConfiguration> action)
      {
         action(Instance);
      }

      public IConfiguration NetworkAvailableCheck(Func<bool> networkCheck)
      {
         Instance.NetworkCheck = networkCheck;
         return this;
      }

      public IConfiguration ConnectTo(string url)
      {
         Instance.Url = url;
         return this;
      }
   }
}