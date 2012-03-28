using System;
using System.Threading;
using NUnit.Framework;

namespace Mashape.Tests
{
   public class BaseFixture
   {
      protected static readonly HttpMethods[] QueryMethods = new[] { HttpMethods.Get };
      protected static readonly HttpMethods[] BodyMethods = new[] { HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete, };
      protected static readonly HttpMethods[] AllMethods = new[] { HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete };

      protected FakeServer Server;
      protected AutoResetEvent Trigger;

      protected virtual bool NeedAServer
      {
         get { return true; }
      }

      [SetUp]
      public void SetUp()
      {
         Utility.NewGuid = () => new Guid("5eee39f1-c096-4693-840a-b61d7d2321af");
         Driver.Configure("leto", "is my her0");
         Driver.Configure(c => c.ConnectTo("http://localhost:" + FakeServer.Port + "/"));
         Trigger = new AutoResetEvent(false);
         if (NeedAServer)
         {
            Server = new FakeServer();
         }
         BeforeEachTest();
      }
      [TearDown]
      public void TearDown()
      {
         Utility.NewGuid = Guid.NewGuid; //kinda sucks
         Driver.Configure(c => c.NetworkAvailableCheck(() => true));
         if (Server != null)
         {
            Server.Dispose();
         }
         AfterEachTest();
      }
      public virtual void AfterEachTest() { }
      public virtual void BeforeEachTest() { }

      protected void SetIfSuccess(Response response)
      {
         if (response.Success) { Set(); }

         Assert.Fail(response.Error.Message);
      }
      protected void Set()
      {
         Trigger.Set();
      }
      protected void WaitOne()
      {
         Assert.IsTrue(Trigger.WaitOne(3000), "Test terminated without properly signalling the trigger");
      }
   }
}