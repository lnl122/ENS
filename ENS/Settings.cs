// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.Win32;

namespace ENS
{
    /// <summary>
    /// реализует работу с настройками, установка и получение
    /// </summary>
    interface ISettings
    {
        void Set(string Name, string Value);
        void Set(string Name, int Value);
        string Get(string Name);
        int GetInt(string Name);
    }

    public class Settings : ISettings, System.IDisposable
    {
        //  лог
        private static Log Log = new Log("Settings");
        private static Registry Reg = new Registry();
        private static bool isReady = false;
        // значения по умолчанию
        private static string[,] default_settings = {
                { "LastUserName", "" },
                { "LastUserPass", "" },
                { "LogFolder", "Log" },
                { "PicsFolder", "Pics" },
                { "DataFolder", "Data" },
                { "PagesFolder", "Pages" },
                { "DatabaseFilename", "data.db" }
            };


        /// <summary>
        /// инит ветки реестра, создание папок при необходимости
        /// </summary>
        public Settings()
        {
            if (!isReady)
            {
                Init();
            }
        }

        /// <summary>
        /// создаем дефолтные значения
        /// </summary>
        private void Init()
        {
            if (!isReady)
            {
                try
                {
                    isReady = true;
                }
                catch
                {
                    Log.Write("ERROR: не удалось создать дефолтные значения");
                    isReady = false;
                }
            }
        }

        /// <summary>
        /// получает значение по умолчанию из таблицы с константами для конкретного параметра по его имени
        /// </summary>
        /// <param name="name">параметр</param>
        /// <returns>значение по умолчанию</returns>
        public string GetDefault(string name)
        {
            string name_lowercase = name.ToLower();
            string value = "";
            int total_settigs = default_settings.Length / 2;
            for(int i = 0; i < total_settigs; i++)
            {
                if (name_lowercase == default_settings[i, 0].ToLower())
                {
                    value = default_settings[i, 1];
                }
            }
            return value;
        }

        /// <summary>
        /// получает значение параметра
        /// </summary>
        /// <param name="name">параметр</param>
        /// <returns>значение</returns>
        public string Get(string name)
        {
            if (!isReady)
            {
                Init();
            }
            string value = Reg.GetValue(name);
            if (value == "")
            {
                value = GetDefault(name);
                Reg.SetValue(name, value);
            }
            return value;
        }

        /// <summary>
        /// получает значение параметра - int
        /// </summary>
        /// <param name="name">параметр</param>
        /// <returns>значение</returns>
        public int GetInt(string name)
        {
            if (!isReady)
            {
                Init();
            }
            string value = Reg.GetValue(name);
            if (value == "")
            {
                value = GetDefault(name);
                Reg.SetValue(name, value);
            }
            int res = -1;
            if (!Int32.TryParse(value, out res))
            {
                res = -1;
            }
            return res;
        }

        /// <summary>
        /// устанавливает значение параметра
        /// </summary>
        /// <param name="Name">имя параметра</param>
        /// <param name="Value">значение</param>
        public void Set(string Name, string Value)
        {
            if (!isReady)
            {
                Init();
            }
            try
            {
                Reg.SetValue(Name, Value);
            }
            catch
            {
                Log.Write("ERROR: не удалось установить значение параметра " + Name);
            }
        }

        /// <summary>
        /// устанавливает значение параметра
        /// </summary>
        /// <param name="Name">имя параметра</param>
        /// <param name="Value">значение</param>
        public void Set(string Name, int Value)
        {
            if (!isReady)
            {
                Init();
            }
            try
            {
                Reg.SetValue(Name, Value);
            }
            catch
            {
                Log.Write("ERROR: не удалось установить значение параметра " + Name);
            }
        }

        /// <summary>
        /// деструктор
        /// </summary>
        public void Dispose()
        {
            isReady = false;
        }
    }
}
