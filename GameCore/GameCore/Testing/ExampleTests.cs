// Copyright 2023 Vivid Helix
// This file is part of ViviHelixTestFramework.
// ViviHelixTestFramework is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// ViviHelixTestFramework is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
// You should have received a copy of the GNU Lesser General Public License along with ViviHelixTestFramework. If not, see <https://www.gnu.org/licenses/>.

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
