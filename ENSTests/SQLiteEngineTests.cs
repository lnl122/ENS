// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ENS;
using System.Collections.Generic;

namespace ENSTests
{
    [TestClass]
    public class SQLiteEngineTests
    {
        [TestMethod]
        public void SQLiteEngine_CorrectDBname()
        {
            using (Settings settings = new Settings())
            {
                Assert.AreEqual("data.db", settings.Get("DatabaseFilename"));
            }
        }
        [TestMethod]
        public void SQLiteEngine_Select()
        {
            using (Settings Options = new Settings())
            {
                string old_db_name = Options.Get("DatabaseFilename");
                Options.Set("DatabaseFilename", "test3.db");
                string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + Options.Get("DatabaseFilename");
                if (File.Exists(DBPath))
                {
                    File.Delete(DBPath);
                }
                Assert.AreEqual(false, File.Exists(DBPath));
                List<List<string>> res1 = new List<List<string>>();
                try
                {
                    using (SQLiteEngine sql = new SQLiteEngine(true))
                    {
                        Assert.AreEqual(true, File.Exists(DBPath));
                        // в БД нужно создать таблицу и записи в ней
                        sql.Query("CREATE TABLE Words ( wrd VARCHAR(50), len INTEGER)");
                        sql.Query("insert into Words (wrd, len) values ('Дятел', 5)");
                        sql.Query("insert into Words (wrd, len) values ('Воробей', 7)");
                        sql.Query("insert into Words (wrd, len) values ('Рыба', 4)");
                        sql.Query("insert into Words (wrd, len) values ('Мамонт', 6)");
                        sql.Query("insert into Words (wrd, len) values ('Корова', 6)");
                        sql.Query("insert into Words (wrd, len) values ('Пес', 3)");
                        sql.Query("insert into Words (wrd, len) values ('Кот', 3)");

                        res1 = sql.Select("SELECT wrd FROM Words WHERE len = 3");
                    }
                }
                catch
                {
                    Options.Set("DatabaseFilename", old_db_name);
                    Assert.AreEqual("", "Исключение при попытке создать объект SQLiteEngine(true) или в создании таблицы/записей или при селекте");
                }
                Options.Set("DatabaseFilename", old_db_name); // "data.db"

                //if (File.Exists(DBPath))
                //{
                //    File.Delete(DBPath);
                //}

                Assert.AreEqual(2, res1.Count);
                List<string> res1_1 = res1[0];
                Assert.AreEqual(1, res1_1.Count);
                List<string> res1_2 = new List<string>();
                res1_2.Add(res1[0][0].ToLower());
                res1_2.Add(res1[1][0].ToLower());
                Assert.AreEqual(true, res1_2.Contains("кот"));
                Assert.AreEqual(true, res1_2.Contains("пес"));
                Assert.AreEqual(false, res1_2.Contains("крокодилище"));
            }
        }
        [TestMethod]
        public void SQLiteEngine_CreateTableInsert()
        {
            using (Settings Options = new Settings())
            {
                string old_db_name = Options.Get("DatabaseFilename");
                Options.Set("DatabaseFilename", "test2.db");
                string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + Options.Get("DatabaseFilename");
                if (File.Exists(DBPath))
                {
                    File.Delete(DBPath);
                }
                Assert.AreEqual(false, File.Exists(DBPath));
                try
                {
                    using (SQLiteEngine sql = new SQLiteEngine(true))
                    {
                        // в БД нужно создать таблицу и записи в ней
                        sql.Query("CREATE TABLE Words ( wrd VARCHAR(50), len INTEGER)");
                        sql.Query("insert into Words (wrd, len) values ('Дятел', 5)");
                        sql.Query("insert into Words (wrd, len) values ('Воробей', 7)");
                        sql.Query("insert into Words (wrd, len) values ('Рыба', 4)");
                        sql.Query("insert into Words (wrd, len) values ('Мамонт', 6)");
                        sql.Query("insert into Words (wrd, len) values ('Корова', 6)");
                        sql.Query("insert into Words (wrd, len) values ('Пес', 3)");
                        sql.Query("insert into Words (wrd, len) values ('Кот', 3)");
                    }
                    Assert.AreEqual(true, File.Exists(DBPath));
                }
                catch
                {
                    Options.Set("DatabaseFilename", old_db_name);
                    Assert.AreEqual("", "Исключение при попытке создать объект SQLiteEngine(true) или в создании таблицы/записей");
                }
                Options.Set("DatabaseFilename", old_db_name); // "data.db"
                //if (File.Exists(DBPath))
                //{
                //    File.Delete(DBPath);
                //}
            }
        }
        [TestMethod]
        public void SQLiteEngine_CreateNew()
        {
            using (Settings settings = new Settings())
            {
                Settings Options = new Settings();
                string old_db_name = Options.Get("DatabaseFilename");
                Options.Set("DatabaseFilename", "test1.db");
                string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + Options.Get("DatabaseFilename");
                if (File.Exists(DBPath))
                {
                    File.Delete(DBPath);
                }
                Assert.AreEqual(false, File.Exists(DBPath));
                try
                {
                    using (SQLiteEngine sql = new SQLiteEngine(true))
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
                    Options.Set("DatabaseFilename", old_db_name);
                    Assert.AreEqual("", "Исключение при попытке создать объект SQLiteEngine(true)");
                }
                Options.Set("DatabaseFilename", old_db_name); // "data.db"
                if (File.Exists(DBPath))
                {
                    File.Delete(DBPath);
                }
            }
        }
        [TestMethod]
        public void SQLiteEngine_OpenExisting()
        {
            using (Settings Options = new Settings())
            {
                string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + Options.Get("DatabaseFilename");
                Assert.AreEqual(true, File.Exists(DBPath));
                //Assert.AreEqual(@"C:\Users\aovsyannikov\source\repos\ENS\ENSTests\bin\Debug\Data\data.db", DBPath);
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
