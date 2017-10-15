// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENS;

namespace ENSTests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void Settings_Read1()
        {
            using (Settings settings = new Settings())
            {
                Assert.AreEqual("data.db", settings.Get("DatabaseFilename"));
                Assert.AreNotEqual("dat345624a.db234", settings.Get("DatabaseFilename"));
                Assert.AreEqual("data.db", settings.Get("databaseFileNAME"));
                Assert.AreNotEqual("dat345624a.db234", settings.Get("databaseFileNAME"));
            }
        }
        [TestMethod]
        public void Settings_ReadDefValue()
        {
            using (Settings settings = new Settings())
            {
                Assert.AreEqual("data.db", settings.Get("DatabaseFilename"));
                Assert.AreNotEqual("dat345624a.db234", settings.Get("DatabaseFilename"));
                Assert.AreEqual("data.db", settings.Get("databaseFileNAME"));
                Assert.AreNotEqual("dat345624a.db234", settings.Get("databaseFileNAME"));
            }
        }
        [TestMethod]
        public void Settings_SetGetValue1()
        {
            using (Settings settings = new Settings())
            {
                settings.Set("SettingsTestValue1", "122122");
                Assert.AreEqual("122122", settings.Get("SettingsTestValue1"));
                Assert.AreEqual(122122, settings.GetInt("SettingsTestValue1"));
                Assert.AreNotEqual("dat345624a.db234", settings.Get("SettingsTestValue1"));
                Assert.AreNotEqual(789465, settings.GetInt("SettingsTestValue1"));
            }
        }
        [TestMethod]
        public void Settings_SetGetValue2()
        {
            using (Settings settings = new Settings())
            {
                settings.Set("SettingsTestValue2", 123);
                Assert.AreEqual("123", settings.Get("SettingsTestValue2"));
                Assert.AreEqual(123, settings.GetInt("SettingsTestValue2"));
                Assert.AreNotEqual("dat345624a.db234", settings.Get("SettingsTestValue2"));
                Assert.AreNotEqual(789465, settings.GetInt("SettingsTestValue2"));
            }
        }
    }
}


