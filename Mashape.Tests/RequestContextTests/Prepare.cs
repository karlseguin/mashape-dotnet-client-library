using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Mashape.Tests.RequestContextTests
{
   public class Prepare : BaseFixture
   {
      protected override bool NeedAServer
      {
         get { return false; }
      }

      [Test]
      public void SetsMetaHeaders()
      {
         var request = new RequestContext(HttpMethods.Get, "anything", Enumerable.Empty<KeyValuePair<string, object>>());
         request.Prepare();
         Assert.AreEqual("dotnet", request.Request.Headers["X-Mashape-Language"]);
         Assert.AreEqual("0.1", request.Request.Headers["X-Mashape-Version"]);
      }

      [Test]
      public void SetsTheProperMethod()
      {
         foreach (var method in AllMethods)
         {
            var request = new RequestContext(method, "anything", Enumerable.Empty<KeyValuePair<string, object>>());
            request.Prepare();
            Assert.AreEqual(method.ToString().ToUpper(), request.Request.Method);
         }
      }

      [Test]
      public void SignsTheRequestUsingTheConfiguredKeys()
      {
         Driver.Configure("pu", "pi");
         var request = new RequestContext(HttpMethods.Get, "anything", Enumerable.Empty<KeyValuePair<string, object>>());
         request.Prepare();
         Assert.AreEqual("cHU6NTVhMzcyNzRhNzU3YTU3YmE0NDZjMDcyMmU2ODhlNWFlNGJiMGUxZTVlZWUzOWYxLWMwOTYtNDY5My04NDBhLWI2MWQ3ZDIzMjFhZg==", request.Request.Headers["X-Mashape-Authorization"]);
      }

      [Test]
      public void QueryMethodsDoesNotPrepareABody()
      {
         foreach(var method in QueryMethods)
         {
            var request = new RequestContext(method, "anything", Enumerable.Empty<KeyValuePair<string, object>>());
            request.Prepare();
            Assert.AreEqual(null, request.Request.ContentType);
            Assert.AreEqual(-1, request.Request.ContentLength);
            Assert.AreEqual(null, request.PayloadData);
         }
      }

      [Test]
      public void PutPostAndDeletePrepareABody()
      {
         foreach (var method in BodyMethods)
         {
            var request = new RequestContext(method, "anything", new Dictionary<string, object>{{"leto", "spice"}});
            request.Prepare();
            Assert.AreEqual("application/x-www-form-urlencoded", request.Request.ContentType);
            Assert.AreEqual(10, request.Request.ContentLength);
            Assert.AreEqual(new byte[] { 108, 101, 116, 111, 61, 115, 112, 105, 99, 101 }, request.PayloadData);
         }
      }

      [Test]
      public void QueryMethodsIncludeTheQueryInTheUrl()
      {
         foreach (var method in QueryMethods)
         {
            var request = new RequestContext(method, "sayans/search", new Dictionary<string, object> { { "itsover", 9000 } });
            request.Prepare();
            Assert.AreEqual("http://localhost:9948/sayans/search?itsover=9000", request.Request.RequestUri.ToString());
         }
      }

      [Test]
      public void BodyMethodsDoNotIncludeTheQueryInTheUrl()
      {
         foreach (var method in BodyMethods)
         {
            var request = new RequestContext(method, "ghola", new Dictionary<string, object> { { "name", "duncan" } });
            request.Prepare();
            Assert.AreEqual("http://localhost:9948/ghola", request.Request.RequestUri.ToString());
         }
      }
   }
}