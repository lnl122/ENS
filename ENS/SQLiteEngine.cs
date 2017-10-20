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
        public static List<string> TablesMustBeList = new List<string>()
            {
                // общий словарь слов
                "Words",
                // словари по длине слова, для оптимизации запросов
                "Words_1_chars", "Words_2_chars", "Words_3_chars", "Words_4_chars", "Words_5_chars", "Words_6_chars", "Words_7_chars", "Words_8_chars", "Words_9_chars",
                "Words_10_chars", "Words_11_chars", "Words_12_chars", "Words_13_chars", "Words_14_chars", "Words_15_chars", "Words_16_chars", "Words_17_chars", "Words_18_chars", "Words_19_chars",
                "Words_20_chars", "Words_21_chars", "Words_22_chars", "Words_23_chars", "Words_24_chars", "Words_25_chars", "Words_26_chars", "Words_27_chars", "Words_28_chars", "Words_29_chars",
                "Words_30_chars", "Words_31_chars", "Words_32_chars", "Words_33_chars", "Words_34_chars", "Words_35_chars", "Words_36_chars", "Words_37_chars", "Words_38_chars", "Words_39_chars",
                "Words_40_chars", "Words_41_chars", "Words_42_chars", "Words_43_chars", "Words_44_chars", "Words_45_chars", "Words_46_chars", "Words_47_chars", "Words_48_chars", "Words_49_char"
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