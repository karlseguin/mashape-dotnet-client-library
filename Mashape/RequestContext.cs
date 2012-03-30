using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Mashape
{
   public class RequestContext
   {
      private readonly HttpMethods _method;
      private readonly IEnumerable<KeyValuePair<string, object>> _payload;
      
      public byte[] PayloadData { get; private set; }
      public HttpWebRequest Request { get; private set; }

      public RequestContext(HttpMethods method, IEnumerable<KeyValuePair<string, object>> payload)
      {
         _method = method;
         _payload = payload;
      }

      public bool UsesQueryString()
      {
         return _method == HttpMethods.Get;
      }

      public void Prepare()
      {
         Request = CreateRequest(_method);
         Request.Headers["X-Mashape-Authorization"] = Utility.CreateSignature(Driver.Instance.PublicKey, Driver.Instance.PrivateKey);
      }

      private HttpWebRequest CreateRequest(HttpMethods method)
      {
         var serializedPayload = Utility.SerializeParameters(_payload);
         var url = Driver.Instance.Url;
         if (UsesQueryString()) { url += '?' + serializedPayload; }
         var request = (HttpWebRequest)WebRequest.Create(url);
         request.Method = method.ToString().ToUpper();
//         request.Headers["X-Mashape-Language"] = "dotnet";
//         request.Headers["X-Mashape-Version"] = Communicator.Version;
#if !WINDOWS_PHONE
         request.Timeout = 10000;
         request.ReadWriteTimeout = 10000;
         request.KeepAlive = false;
#endif
         if (!UsesQueryString())
         {
            PayloadData = PreparePost(request, serializedPayload);
         }
         return request;
      }

      private static byte[] PreparePost(HttpWebRequest request, string serializedPayload)
      {
         request.ContentType = "application/x-www-form-urlencoded";
         var data = Encoding.UTF8.GetBytes(serializedPayload);
#if !WINDOWS_PHONE
         request.ContentLength = data.Length;
#endif
         return data;
      }
   }

   public class RequestContext<T> : RequestContext
   {
      public RequestContext(HttpMethods method, IEnumerable<KeyValuePair<string, object>> payload, Action<Response<T>> callback) : base(method, payload)
      {
         Callback = callback;
      }

      public Action<Response<T>> Callback { get; private set; }
      internal Func<JObject, T> Deserializer { get; set; }
   }
}