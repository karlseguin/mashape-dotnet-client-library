namespace Mashape
{
   public class Response
   {
      public bool Success { get; internal set; }
      internal string Raw { get; set; }
      public MashapeException Error { get; internal set; }
   }
   public class Response<T> : Response
   {
      public T Data { get; internal set; }

      public static Response<T> CreateSuccess(string raw)
      {
         return new Response<T> { Success = true, Raw = raw };
      }
      public static Response<T> CreateError(MashapeException error)
      {
         return new Response<T> { Success = false, Error = error };
      }
   }
}