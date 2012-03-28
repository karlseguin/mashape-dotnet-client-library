using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Mashape
{
   public static class Utility
   {
      public static Func<Guid> NewGuid = () => Guid.NewGuid();

      public static string SerializeParameters(IEnumerable<KeyValuePair<string, object>> payload)
      {
         var sb = new StringBuilder();
         foreach (var kvp in payload)
         {
            if (kvp.Value == null) { continue; }
            var valueType = kvp.Value.GetType();
            if (!typeof(string).IsAssignableFrom(valueType) && typeof(IEnumerable).IsAssignableFrom(valueType))
            {
               sb.Append(Serialize(kvp.Key, (IEnumerable)kvp.Value));
            }
            else
            {
               sb.Append(SerializeSingleParameter(kvp.Key, kvp.Value.ToString()));
            }
         }
         return sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : string.Empty;
      }

      private static string Serialize(string key, IEnumerable values)
      {
         var sb = new StringBuilder();
         key = string.Concat(key, "[]");
         foreach (var value in values)
         {
            sb.Append(SerializeSingleParameter(key, value.ToString()));
         }
         return sb.ToString();
      }

      private static string SerializeSingleParameter(string key, string value)
      {
         return string.Concat(key, '=', Uri.EscapeDataString(value), '&');
      }

      public static string CreateSignature(string publicKey, string privateKey)
      {
         var uuid = NewGuid().ToString();
         var hash = CreateHash(uuid, privateKey);
         var raw = string.Format("{0}:{1}{2}", publicKey, hash, uuid);
         return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
      }

      private static string CreateHash(string uuid, string privateKey)
      {
         using (var hasher = new HMACSHA1(Encoding.UTF8.GetBytes(privateKey)))
         {
            var bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(uuid));
            var data = new StringBuilder(bytes.Length * 2);
            for (var i = 0; i < bytes.Length; ++i)
            {
               data.Append(bytes[i].ToString("x2"));
            }
            return data.ToString();
         }
      }
   }
}