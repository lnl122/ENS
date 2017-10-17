// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;

namespace ENS
{
    public class FilePath
    {
        /// <summary>
        /// если папка есть, или если не было, но удалось создать - возвращает путь к ней, иначе - базовый путь
        /// </summary>
        /// <param name="basepath">базовый путь</param>
        /// <param name="folder">имя папки</param>
        /// <returns>путь к папке</returns>
        public static string CheckCreateFolder(string folder)
        {
            string basepath = Environment.CurrentDirectory;
            string path = basepath + @"\" + folder.Replace("\\", "");
            if (!System.IO.Directory.Exists(path))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                catch
                {
                    path = basepath;
                }
            }
            return path + "\\";
        }

        //// здесь и ниже - передаем установленные значения путей/файлов
        //public static string GetLogFilePath()
        //{
        //    Settings Options = new Settings();
        //    return CheckCreateFolder(Options.Get("LogFolder")) + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName + ".log"; ;
        //}
        //public static string GetPagesFolder()
        //{
        //    Settings Options = new Settings();
        //    return CheckCreateFolder(Options.Get("PagesFolder"));
        //}
        //public static string GetPicsFolder()
        //{
        //    Settings Options = new Settings();
        //    return CheckCreateFolder(Options.Get("PicsFolder"));
        //}
        //public static string GetDataFolder()
        //{
        //    Settings Options = new Settings();
        //    return CheckCreateFolder(Options.Get("DataFolder"));
        //}

    }
}
