// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENS;

namespace ENSTests
{
    //[TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreNotEqual(-1, Test1.FuncAdd(2, 3));
        }
        [TestMethod]
        public void TestMethod2()
        {
            Assert.AreEqual(5, Test1.FuncAdd(2, 3));
        }
    }


}
