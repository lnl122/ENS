// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ENS;


namespace ENSTests
{
    [TestClass]
    public class SQLiteEngineTests
    {
        [TestMethod]
        public void SQLiteEngine_CreateTableInsert()
        {
            using (Settings settings = new Settings())
            {
                Settings Options = new Settings();
                string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + "test.db";
                if (File.Exists(DBPath))
                {
                    File.Delete(DBPath);
                }
                try
                {
                    using (SQLiteEngine sql = new SQLiteEngine(true))
                    {
                        sql.Query(" ");
                    }
                    if (File.Exists(DBPath))
                    {
                        File.Delete(DBPath);
                    }
                }
                catch
                {
                    Assert.AreEqual("", "Исключение при попытке создать объект SQLiteEngine(true)");
                }
            }
        }
        [TestMethod]
        public void SQLiteEngine_CreateNew()
        {
            using (Settings settings = new Settings())
            {
                Settings Options = new Settings();
                string old_db_name = Options.Get("DatabaseFilename");
                Options.Set("DatabaseFilename", "test.db");
                string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + Options.Get("DatabaseFilename");
                if (File.Exists(DBPath))
                {
                    File.Delete(DBPath);
                }
                Assert.AreEqual(false, File.Exists(DBPath));
                try
                {
                    using (SQLiteEngine En = new SQLiteEngine(true))
                    {
                        Assert.AreEqual(true, File.Exists(DBPath));
                    }
                    if (File.Exists(DBPath))
                    {
                        File.Delete(DBPath);
                    }
                }
                catch
                {
                    Assert.AreEqual("", "Исключение при попытке создать объект SQLiteEngine(true)");
                }
                Options.Set("DatabaseFilename", old_db_name); // "data.db"
            }
        }
        [TestMethod]
        public void SQLiteEngine_OpenExisting()
        {
            using (Settings settings = new Settings())
            {
                Settings Options = new Settings();
                string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + Options.Get("DatabaseFilename");
                Assert.AreEqual(true, File.Exists(DBPath));
                try
                {
                    using (SQLiteEngine En = new SQLiteEngine())
                    {
                        Assert.AreEqual(true, File.Exists(DBPath));
                    }
                }
                catch
                {
                    Assert.AreEqual("", "Исключение при попытке создать объект SQLiteEngine(true)");
                }
            }
        }
    }
}
