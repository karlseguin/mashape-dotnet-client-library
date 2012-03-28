using System;
using NUnit.Framework;

namespace Mashape.Tests.UtilityTests
{
   public class CreateSignature : BaseFixture
   {
      [Test]
      public void CreateAValieSignature()
      {
         Utility.NewGuid = () => new Guid("aeee39f1-c096-4693-840a-b61d7d2321a0");
         var signature = Utility.CreateSignature("over", "9000!");
         Assert.AreEqual("b3ZlcjozY2ZjYmZlZGRmOWU2MzRhY2RhYzFmMTg4ZjcwZGRmMzI0MDAzNDBjYWVlZTM5ZjEtYzA5Ni00NjkzLTg0MGEtYjYxZDdkMjMyMWEw", signature);
      }
   }
}