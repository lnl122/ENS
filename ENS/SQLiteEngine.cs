// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace ENS
{
    /// <summary>
    /// реализует работу с SQLite базой
    /// </summary>
    interface ISQLiteEngine
    {
        void Query(string text);
        List<List<string>> SelectTable(string text);
        List<string> SelectColumn(string text);
        bool Check();
    }

    public class SQLiteEngine : ISQLiteEngine, IDisposable
    {
        // прризнак проведенной инициализации
        public static bool isReady = false;
        // объекты блокировки
        private static object LockWrite = new object();
        private static object LockInit = new object();
        // лог
        private static Log Log = new Log("SQLiteEngine");
        // параметры SQLite
        private static SQLiteConnection connection;

        // перечень таблиц, которые обязательно должны быть в базе
        List<string> TablesMustBeList = new List<string>()
            {
                // общий словарь слов
                "Words",
                // словари по длине слова, для оптимизации запросов
                "Words_1_char", "Words_2_char", "Words_3_char", "Words_4_char", "Words_5_char", "Words_6_char", "Words_7_char", "Words_8_char", "Words_9_char",
                "Words_10_char", "Words_11_char", "Words_12_char", "Words_13_char", "Words_14_char", "Words_15_char", "Words_16_char", "Words_17_char", "Words_18_char", "Words_19_char",
                "Words_20_char", "Words_21_char", "Words_22_char", "Words_23_char", "Words_24_char", "Words_25_char", "Words_26_char", "Words_27_char", "Words_28_char", "Words_29_char",
                "Words_30_char", "Words_31_char", "Words_32_char", "Words_33_char", "Words_34_char", "Words_35_char", "Words_36_char", "Words_37_char", "Words_38_char", "Words_39_char",
                "Words_40_char", "Words_41_char", "Words_42_char", "Words_43_char", "Words_44_char", "Words_45_char", "Words_46_char", "Words_47_char", "Words_48_char", "Words_49_char"
            };

        /// <summary>
        /// инициализирует соединение с базой
        /// </summary>
        public SQLiteEngine(bool needCreate = false)
        {
            if (!isReady)
            {
                lock (LockInit)
                {
                    if (!isReady)
                    {
                        Settings Options = new Settings();
                        string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + Options.Get("DatabaseFilename");
                        string its_new = "False";
                        if (needCreate)
                        {
                            its_new = "True";
                        }
                        try
                        {
                            connection = new SQLiteConnection("Data Source=" + DBPath + ";Version=3;New=" + its_new + ";Compress=True;");
                            connection.Open();
                            isReady = true;
                        }
                        catch
                        {
                            isReady = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// выполняет запрос в БД, не возвращающий таблицы значений (create table/insert/update/delete)
        /// </summary>
        /// <param name="text">текст запроса</param>
        public void Query(string text)
        {
            if (isReady)
            {
                lock (LockWrite)
                {
                    try
                    {
                        SQLiteCommand command = new SQLiteCommand(text, connection);
                        //SQLiteCommand command = connection.CreateCommand();
                        //command.CommandText = text;
                        command.ExecuteNonQuery();
                    }
                    catch
                    {
                        Log.Write("ошибка при выполнении запроса к БД (create table/insert/update/delete)");
                    }
                }
            }
        }

        /// <summary>
        /// выполняет запрос в БД, возвращающий таблицу значений (select)
        /// </summary>
        /// <param name="text">текст запроса</param>
        /// <returns>список строк, содержащий список колонок со значениями в string</returns>
        public List<List<string>> SelectTable(string text)
        {
            List<List<string>>  res = new List<List<string>>();
            if (isReady)
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(text, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    int columns = reader.FieldCount;
                    while (reader.Read())
                    {
                        List<string> one_row = new List<string>();
                        for (int i = 0; i < columns; i++)
                        {
                            one_row.Add(reader.GetString(i));
                        }
                        res.Add(one_row);
                    }
                }
                catch
                {
                    Log.Write("ошибка при выполнении запроса к БД, возвращающего таблицу (select)");
                }
            }
            return res;
        }

        /// <summary>
        /// выполняет запрос в БД, возвращающий таблицу значений (select)
        /// </summary>
        /// <param name="text">текст запроса</param>
        /// <returns>список со значениями в string</returns>
        public List<string> SelectColumn(string text)
        {
            List<string> res = new List<string>();
            if (isReady)
            {
                try
                {
                    SQLiteCommand command = new SQLiteCommand(text, connection);
                    SQLiteDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        res.Add(reader.GetString(0));
                    }
                }
                catch
                {
                    Log.Write("ошибка при выполнении запроса к БД, возвращающего таблицу (select)");
                }
            }
            return res;
        }

        /// <summary>
        /// деструктор, закрывает соединение с БД
        /// </summary>
        public void Dispose()
        {
            connection.Close();
            isReady = false;
        }

        /// <summary>
        /// Проверяет наличие всех необходимых для работы таблиц в БД
        /// </summary>
        /// <returns>флаг корректности проверки</returns>
        public bool Check()
        {
            bool res = true;
            try
            {
                List<string> ListOfTables = SelectColumn("SELECT name FROM sqlite_master WHERE type = 'table'");
                foreach (string table in TablesMustBeList)
                {
                    if (!ListOfTables.Contains(table))
                    {
                        res = false;
                    }
                }
            }
            catch
            {
                Log.Write("ошибка при выполнении наличия необходимых таблиц БД");
            }
            return res;
        }
    }
}

//const string query = "Select * From Invoice Where InvNumber = @InvNumber";
//using (SqlCommand cmd = new SqlCommand(query, conn))
//cmd.Parameters.Add(new SqlParameter("@InvNumber", 1100));