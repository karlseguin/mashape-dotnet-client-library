using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mashape
{
   public class Communicator
   {
      public const string Version = "V01";
      private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore, };

#if !WINDOWS_PHONE
      public static T SendPayload<T>(RequestContext context)
      {
         return SendPayload<T>(context, null);
      }
      public static T SendPayload<T>(RequestContext context, Func<JObject, T> deserializer)
      {
         if (Driver.Instance.NetworkCheck != null && !Driver.Instance.NetworkCheck())
         {
            throw new MashapeException { Message = "Network is not available" };
         }
         try
         {
            context.Prepare();
            if (!context.UsesQueryString())
            {
               using (var stream = context.Request.GetRequestStream())
               {
                  stream.Write(context.PayloadData, 0, context.PayloadData.Length);
                  stream.Flush();
                  stream.Close();
               }
            }
            using (var response = (HttpWebResponse)context.Request.GetResponse())
            {
               var body = GetResponseBody(response);
               if (string.IsNullOrEmpty(body))
               {
                  return default(T);
               }
               if (deserializer == null)
               {
                  return JsonConvert.DeserializeObject<T>(body);
               }
               return deserializer(JObject.Parse(body));
            }
         }
         catch (Exception e)
         {
            throw HandleException(e);
         }
      }
#endif
      public static void SendPayload<T>(RequestContext<T> context)
      {
         SendPayload(context, null);
      }

      public static void SendPayload<T>(RequestContext<T> context, Func<JObject, T> deserializer)
      {
         if (Driver.Instance.NetworkCheck != null && !Driver.Instance.NetworkCheck())
         {
            if (context.Callback != null) { context.Callback(Response<T>.CreateError(new MashapeException { Message = "Network is not available" })); }
            return;
         }
         context.Deserializer = deserializer;
         context.Prepare();
         if (context.UsesQueryString())
         {
            context.Request.BeginGetResponse(GetResponseStream<T>, context);
         }
         else
         {
            context.Request.BeginGetRequestStream(GetRequestStream<T>, context);
         }
      }

      private static void GetRequestStream<T>(IAsyncResult result)
      {
         var context = (RequestContext)result.AsyncState;
         using (var requestStream = context.Request.EndGetRequestStream(result))
         {
            requestStream.Write(context.PayloadData, 0, context.PayloadData.Length);
            requestStream.Flush();
            requestStream.Close();
         }
         context.Request.BeginGetResponse(GetResponseStream<T>, context);
      }

      private static void GetResponseStream<T>(IAsyncResult result)
      {
         var context = (RequestContext<T>)result.AsyncState;
         try
         {
            using (var response = (HttpWebResponse)context.Request.EndGetResponse(result))
            {
               if (context.Callback != null)
               {
                  var r = Response<T>.CreateSuccess(GetResponseBody(response));
                  if (context.Deserializer == null)
                  {
                     r.Data = JsonConvert.DeserializeObject<T>(r.Raw);
                  }
                  else
                  {
                     r.Data = context.Deserializer(new JObject(r.Raw));
                  }
                  context.Callback(r);
               }
            }
         }
         catch (Exception ex)
         {
            if (context.Callback != null) { context.Callback(Response<T>.CreateError(HandleException(ex))); }
         }
      }

      private static string GetResponseBody(WebResponse response)
      {
         using (var stream = response.GetResponseStream())
         {
            var sb = new StringBuilder();
            int read;
            var bufferSize = response.ContentLength == -1 ? 4096 : (int)response.ContentLength;
            if (bufferSize == 0) { return null; }
            do
            {
               var buffer = new byte[4096];
               read = stream.Read(buffer, 0, buffer.Length);
               sb.Append(Encoding.UTF8.GetString(buffer, 0, read));
            } while (read > 0);
            return sb.ToString();
         }
      }

      private static MashapeException HandleException(Exception exception)
      {
         if (exception is WebException)
         {
            var response = ((WebException)exception).Response;
            if (response == null)
            {
               return new MashapeException { Message = "Null response", InnerException = exception, Code = -1 };
            }
            var body = GetResponseBody(response);
            try
            {
               var message = JsonConvert.DeserializeObject<MashapeException>(body, _jsonSettings);
               message.InnerException = exception;
               return message;
            }
            catch (Exception)
            {
               return new MashapeException { Message = body, InnerException = exception, Code = -2 };
            }
         }
         return new MashapeException { Message = "Unknown Error", InnerException = exception, Code = -3 };
      }


   }
}