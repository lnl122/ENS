// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENS;

namespace ENSTests
{
    //[TestClass]
    public class RegistryTests
    {
        [TestMethod]
        public void Reg_ReadNonExistsValue()
        {
            using (Registry reg = new Registry())
            {
                Assert.AreEqual("", reg.GetValue("sfghsfghstfhwvrthdfh555"));
            }
        }
        [TestMethod]
        public void Reg_WriteReadStringValue()
        {
            using (Registry reg = new Registry())
            {
                reg.SetValue("TestValueString", "abc123");
                Assert.AreEqual("abc123", reg.GetValue("TestValueString"));
                Assert.AreNotEqual("a2bc123ff", reg.GetValue("TestValueString"));
                Assert.AreNotEqual("abc123", reg.GetValue("TestValueStringIllegal"));
            }
        }
        [TestMethod]
        public void Reg_WriteReadIntValue()
        {
            using (Registry reg = new Registry())
            {
                reg.SetValue("TestValueInt", 123);
                Assert.AreEqual(123, reg.GetValueInt("TestValueInt"));
                Assert.AreNotEqual(321321, reg.GetValueInt("TestValueInt"));
                Assert.AreNotEqual(123, reg.GetValueInt("TestValueIntIllegal"));
            }
        }
    }
}


