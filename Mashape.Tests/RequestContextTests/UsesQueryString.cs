using System.Linq;
using NUnit.Framework;

namespace Mashape.Tests.RequestContextTests
{
   public class UsesQueryString : BaseFixture
   {
      protected override bool NeedAServer
      {
         get { return false; }
      }

      [Test]
      public void QueryMethodsUseTheQueryString()
      {
         Assert.IsTrue(QueryMethods.All(method => new RequestContext(method, null, null).UsesQueryString()));
      }

      [Test]
      public void BodyMethodsDoNotUseAQueryString()
      {
         Assert.IsTrue(BodyMethods.All(method => !new RequestContext(method, null, null).UsesQueryString()));
      }
   }
}