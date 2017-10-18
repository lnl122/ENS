// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ENS
{
    public static class Program
    {
        // лог
        private static Log Log = new Log("Program");

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // инициализируемся
            Log.Write("Запустили ENS");

            using (Settings Options = new Settings())
            {
                string old_db_name = Options.Get("DatabaseFilename");
                Options.Set("DatabaseFilename", "test3.db");
                string DBPath = FilePath.CheckCreateFolder(Options.Get("DataFolder")) + Options.Get("DatabaseFilename");
                List<List<string>> res1 = new List<List<string>>();
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

                        res1 = sql.Select("SELECT wrd FROM Words WHERE len = 3");
                    }
                }
                catch
                {
                    Options.Set("DatabaseFilename", old_db_name);
                }
                Options.Set("DatabaseFilename", old_db_name); // "data.db"

                List<string> res1_1 = res1[0];
                List<string> res1_2 = new List<string>();
                res1_2.Add(res1[0][0].ToLower());
                res1_2.Add(res1[1][0].ToLower());
                int tt = 0;

            }

            // запускаем необходимые компоненты (вбиватор, сервер, клиент, решалка, )
            Console.WriteLine(Test1.FuncAdd(2, 3));

            // завершаем работу
            Log.Write("Закончили работу ENS");
            Log.Close();
        }
    }

    /// <summary>
    /// тестовый класс
    /// </summary>
    public static class Test1
    {
        /// <summary>
        /// тестовая функция для отладки NUnit
        /// </summary>
        /// <param name="a">1 слагаемое</param>
        /// <param name="b">2 слагаемое</param>
        /// <returns>сумма двух чисел</returns>
        public static int FuncAdd(int a, int b)
        {
            return a + b;
        }
    }
}
