using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENS;

namespace ENS_CreateDatabase
{
    class Program
    {
        // новая база
        public static SQLiteEngine sql = new SQLiteEngine(true);

        static void Main(string[] args)
        {
            //sql = new SQLiteEngine(true);
            string folder = @"..\..\..\Database_source\";

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            LoadWordsDictionary(folder + "words.txt");

            stopWatch.Stop();
            WriteTimeSpan("reading dictionary time =      ", stopWatch.Elapsed);
            //"Words", "Words_49_char"

            System.Diagnostics.Stopwatch stopWatch1 = new System.Diagnostics.Stopwatch();
            stopWatch1.Start();

            DivideWordsByLength();

            stopWatch1.Stop();
            WriteTimeSpan("div words by length =          ", stopWatch1.Elapsed);
            

            if (sql.Check())
            {
                Console.WriteLine("Не все таблицы присутствуют в БД!");
            }
            else
            {
                Console.WriteLine("Все необходимые таблицы созданы. необходимо скопировать файл БД в папку с ENS");
            }
            Console.WriteLine("\r\n\r\nНажмите Enter для выхода...");
            Console.ReadLine();
        }

        /// <summary>
        /// читает словарь из файла в базу
        /// </summary>
        /// <param name="v">файл с разделителем слов - пробелом</param>
        private static void LoadWordsDictionary(string v)
        {
            if (System.IO.File.Exists(v))
            {
                sql.Query("DROP TABLE IF EXISTS Words");

                System.IO.StreamReader dict = new System.IO.StreamReader(v, Encoding.Unicode);
                List<string> dict_lst = dict.ReadToEnd().Split(' ').ToList();

                sql.Query("CREATE TABLE Words ( wrd VARCHAR(50), len INTEGER)");
                sql.Query("BEGIN TRANSACTION");
                foreach (string wrd in dict_lst)
                {
                    string wrd2 = wrd.ToLower().Trim().Replace("'", "''");
                    if (wrd2.Length > 1)
                    {
                        sql.Query("INSERT INTO Words (wrd, len) VALUES ('" + wrd2 + "', " + wrd2.Length + ")");
                    }
                }
                sql.Query("COMMIT TRANSACTION");
            }
        }        
        
        /// <summary>
        /// разносит словарь по таблицам слов с фиксированной длинной
        /// </summary>
        private static void DivideWordsByLength()
        {
            sql.Query("BEGIN TRANSACTION");
            foreach (string table in SQLiteEngine.TablesMustBeList)
            {
                if (table.Contains("Words") && table.Contains("_chars"))
                {
                    sql.Query("DROP TABLE IF EXISTS " + table);
                }
            }

            for (int i = 1; i < 50; i++)
            {
                sql.Query("CREATE TABLE Words_" + i.ToString() + "_chars ( wrd VARCHAR(50) )");
                sql.Query("INSERT INTO Words_" + i.ToString() + "_chars ( wrd ) SELECT wrd FROM Words WHERE len = " + i.ToString());
            }
            sql.Query("COMMIT TRANSACTION");
        }

        /// <summary>
        /// вывести в консоль время выполнения
        /// </summary>
        /// <param name="str">текст</param>
        /// <param name="ts">TimeSpan</param>
        public static void WriteTimeSpan(string str, TimeSpan ts)
        {
            Console.WriteLine(str + String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds));
        }
    }
}
