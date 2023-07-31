using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace GameCore.Testing
{
    [TestFixture]
    [Category("CoreTests")]
    public class ExampleTests
    {
        [Test]
        public void TestEmptyPassingTest()
        {
            Assert.Pass();
        }

        [Test]
        public void TestEmptyFailinggTest()
        {
            Assert.Fail();
        }
    }

    [TestFixture]
    [Category("AlternateTests")]
    public class AlternateExampleTests
    {
        [Test]
        public void TestAltPassingTest()
        {
            Assert.Pass();
        }
    }

}
