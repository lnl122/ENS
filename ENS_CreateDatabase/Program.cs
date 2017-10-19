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
            sql = new SQLiteEngine(true);
            string folder = @"C:\Users\aovsyannikov\source\repos\ENS\_database_source\";

            LoadWordsDictionary(folder + "dict.txt");
            //"Words", "Words_49_char"


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

        private static void LoadWordsDictionary(string v)
        {
            throw new NotImplementedException();
        }
    }
}
