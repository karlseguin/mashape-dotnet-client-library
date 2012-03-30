using System;
using Newtonsoft.Json.Linq;

namespace Mashape
{
   public static class SpecialDeserializers
   {
       public static Func<JObject, T> SingleValue<T>(string path)
       {
          return j => j.SelectToken(path).ToObject<T>();
       }
   }
}