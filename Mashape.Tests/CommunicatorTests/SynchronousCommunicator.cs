using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;

namespace Mashape.Tests.CommunicatorTests
{
   public class SynchronousCommunicator : BaseFixture
   {
      [Test]
      public void ThrowsExceptionIfNetworkIsntAvailable()
      {
         Driver.Configure(c => c.NetworkAvailableCheck(() => false));
         var ex = Assert.Throws<MashapeException>(() => Communicator.SendPayload<object>(new RequestContext(HttpMethods.Get, null, null)));
         Assert.AreEqual("Network is not available", ex.Message);
      }

      [Test]
      public void SendsMessageToServer()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/scores/count", Request = "lid=theid&scope=2", Response = "{}" });
         Communicator.SendPayload<object>(new RequestContext(HttpMethods.Get, "scores/count", new Dictionary<string, object>{{"lid", "theid"}, {"scope", 2}}));
      }

      [Test]
      public void SendsAPostMessageToServer()
      {
         Server.Stub(new ApiExpectation { Method = "Post", Url = "/scores", Request = "lid=theid&score=44", Response = "{}" });
         Communicator.SendPayload<object>(new RequestContext(HttpMethods.Post, "scores", new Dictionary<string, object> { { "lid", "theid" }, { "score", 44} }));
      }

      [Test]
      public void GetsResponseFromServer()
      {
         Server.Stub(new ApiExpectation { Response = "{name: 'goku'}" });
         var response = Communicator.SendPayload<AResponse>(new RequestContext(HttpMethods.Get, "sayans", new Dictionary<string, object> { { "id", "1" } }));
         Assert.AreEqual("goku", response.Name);
      }

      [Test]
      public void ThrowsExceptionOnDeserializationErrors()
      {
         Server.Stub(new ApiExpectation { Response = "invalid" });
         var ex = Assert.Throws<MashapeException>(() => Communicator.SendPayload<AResponse>(new RequestContext(HttpMethods.Get, "sayans", Enumerable.Empty<KeyValuePair<string, object>>())));
         Assert.AreEqual("Unknown Error", ex.Message);
         Assert.IsAssignableFrom<Newtonsoft.Json.JsonReaderException>(ex.InnerException);
      }

      [Test]
      public void ThrowsExceptionOnServerError()
      {
         Server.Stub(new ApiExpectation { Request = "Something Different"});
         var ex = Assert.Throws<MashapeException>(() => Communicator.SendPayload<AResponse>(new RequestContext(HttpMethods.Get, "sayans", Enumerable.Empty<KeyValuePair<string, object>>())));
         Assert.AreEqual("Unexpected call: GET http://localhost:9948/sayans?\r\n", ex.Message);
         Assert.IsAssignableFrom<WebException>(ex.InnerException);
      }


      public class AResponse
      {
         public string Name { get; set; }
      }
   }
}