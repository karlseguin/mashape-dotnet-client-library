using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;

namespace Mashape.Tests.CommunicatorTests
{
   public class AsynchronousCommunicator : BaseFixture
   {
      [Test]
      public void ReturnsAFailureIfNetworkIsntAvailable()
      {
         Driver.Configure(c => c.NetworkAvailableCheck(() => false));
         Communicator.SendPayload(new RequestContext<object>(HttpMethods.Get, null, r =>
         {
            Assert.AreEqual(false, r.Success);
            Assert.AreEqual("Network is not available", r.Error.Message);
            Set();
         }));
         WaitOne();
      }

      [Test]
      public void SendsMessageToServer()
      {
         Server.Stub(new ApiExpectation { Method = "GET", Url = "/api.spice", Request = "lid=theid&scope=2", Response = "{}" });
         Communicator.SendPayload(new RequestContext<object>(HttpMethods.Get, new Dictionary<string, object> { { "lid", "theid" }, { "scope", 2 } }, r =>
         {
            Assert.IsTrue(r.Success);
            Set();
         })); 
         WaitOne();
      }

      [Test]
      public void SendsAPostMessageToServer()
      {
         Server.Stub(new ApiExpectation { Method = "Post", Url = "/api.spice", Request = "lid=theid&score=44", Response = "{}" });
         Communicator.SendPayload(new RequestContext<object>(HttpMethods.Post, new Dictionary<string, object> { { "lid", "theid" }, { "score", 44 } }, r =>
         {
            Assert.IsTrue(r.Success);
            Set();
         }));
         WaitOne();
      }

      [Test]
      public void GetsResponseFromServer()
      {
         Server.Stub(new ApiExpectation { Response = "{name: 'goku'}" });
         Communicator.SendPayload(new RequestContext<AResponse>(HttpMethods.Get, new Dictionary<string, object> { { "id", "1" } }, r =>
         {
            Assert.IsTrue(r.Success);
            Assert.AreEqual("goku", r.Data.Name);
            Set();
         }));
         WaitOne();
         
      }

      [Test]
      public void ThrowsExceptionOnDeserializationErrors()
      {
         Server.Stub(new ApiExpectation { Response = "invalid" });
         Communicator.SendPayload(new RequestContext<AResponse>(HttpMethods.Get, Enumerable.Empty<KeyValuePair<string, object>>(), r =>
         {
            Assert.AreEqual("Unknown Error", r.Error.Message);
            Assert.IsAssignableFrom<Newtonsoft.Json.JsonReaderException>(r.Error.InnerException);
            Set();
         }));
         WaitOne();
      }

      [Test]
      public void ThrowsExceptionOnServerError()
      {
         Server.Stub(new ApiExpectation { Request = "Something Different" });
         Communicator.SendPayload(new RequestContext<AResponse>(HttpMethods.Get, Enumerable.Empty<KeyValuePair<string, object>>(), r =>
         {
            Assert.AreEqual("Unexpected call: GET http://localhost:9948/api.spice?\r\n", r.Error.Message);
            Assert.IsAssignableFrom<WebException>(r.Error.InnerException);
            Set();
         }));
         WaitOne();
      }


      public class AResponse
      {
         public string Name { get; set; }
      }
   }
}