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
