using System;
using Newtonsoft.Json;

namespace Mashape
{
   public class MashapeException : Exception
   {
      [JsonProperty("error")]
      public string Message { get; internal set; }
      public Exception InnerException { get; internal set; }
   }
}