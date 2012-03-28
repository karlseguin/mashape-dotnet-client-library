using System.Collections.Generic;
using NUnit.Framework;

namespace Mashape.Tests.UtilityTests
{
   public class SeiralizeParameters : BaseFixture
   {
      [Test]
      public void PayloadProperlyHandlesIEnumerables()
      {
         Server.Stub(new ApiExpectation{Response = "{}"});
         Server.OnInvoke(c =>
         {
            Assert.AreEqual("2,5,6", c.Request.QueryString["scopes[]"]);
            Set();
         });
         var request = new RequestContext(HttpMethods.Get, "anything", new Dictionary<string, object> {{"scopes", new[] {2, 5, 6}}});
         Communicator.SendPayload<object>(request);
         WaitOne();
      }

      [Test]
      public void PayloadPropertyEncodesValues()
      {
         Server.Stub(new ApiExpectation { Response = "{}" });
         Server.OnInvoke(c =>
         {
            Assert.IsTrue(c.Request.Url.Query.Contains("data=2%20%2B%203%20%3D%205"));
            Set();
         });
         var request = new RequestContext(HttpMethods.Get, "anything", new Dictionary<string, object> { { "data", "2 + 3 = 5" } });
         Communicator.SendPayload<object>(request);
         WaitOne();
      }
   }
}