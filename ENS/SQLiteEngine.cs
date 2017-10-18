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
        List<List<string>> Select(string text);
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
        public List<List<string>> Select(string text)
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
                List<List<string>> ListOfTables = Select("");

            }
            catch
            {
                Log.Write("ошибка при выполнении наличия необходимых таблиц БД");
            }
            return true;
        }
    }
}

//const string query = "Select * From Invoice Where InvNumber = @InvNumber";
//using (SqlCommand cmd = new SqlCommand(query, conn))
//cmd.Parameters.Add(new SqlParameter("@InvNumber", 1100));