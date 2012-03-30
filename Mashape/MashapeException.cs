using System;
using Newtonsoft.Json;

namespace Mashape
{
   public class MashapeException : Exception
   {
      [JsonProperty("code")]
      public int Code { get; internal set; }
      [JsonProperty("message")]
      public string Message { get; internal set; }
      public Exception InnerException { get; internal set; }
   }
}