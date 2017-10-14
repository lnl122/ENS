// Copyright © 2017 Antony S. Ovsyannikov aka lnl122
// License: http://opensource.org/licenses/MIT

using System;
using Microsoft.Win32;

namespace ENS
{
    /// <summary>
    /// реализует работу с реестром - получение сведений о версии .NET, get/set user/pass
    /// </summary>
    interface IRegistry
    {
        //string GetVersionDotNet();
        void SetValue(string KeyName, string Value);
        void SetValue(string KeyName, int Value);
        string GetValue(string KeyName);
        int GetValueInt(string KeyName);
    }

    public class Registry : IRegistry, System.IDisposable
    {
        //  лог
        private static Log Log = new Log("Registry");
        private static bool isReady = false;
        // константы
        private const string HCKU_lev1 = "Software";
        private const string HCKU_lev2 = "lnl122";
        private const string HCKU_lev3 = "ENS";

        /// <summary>
        /// инит ветки реестра, создание папок при необходимости
        /// </summary>
        public Registry()
        {
            if (!isReady)
            {
                Init();
            }
        }

        /// <summary>
        /// создаем при необходимости ветки реестра для учетки юзера
        /// </summary>
        private void Init()
        {
            if (!isReady)
            {
                try
                {
                    RegistryKey rk = Microsoft.Win32.Registry.CurrentUser;
                    RegistryKey rks = rk.OpenSubKey(HCKU_lev1, true);
                    rk.Close();
                    RegistryKey rksl = rks.OpenSubKey(HCKU_lev2, true);
                    if (rksl == null)
                    {
                        rksl = rks.CreateSubKey(HCKU_lev2);
                    }
                    rks.Close();
                    RegistryKey rksls = rksl.OpenSubKey(HCKU_lev3, true);
                    if (rksls == null)
                    {
                        rksls = rksl.CreateSubKey(HCKU_lev3);
                    }
                    rksl.Close();
                    rksls.Close();
                    isReady = true;
                }
                catch
                {
                    Log.Write("ERROR: не удалось создать корень ветки реестра для программы в целом. может быть нет прав?");
                    isReady = false;
                }
            }
        }

        /// <summary>
        /// возвращает строку с указанием версий .net
        /// </summary>
        /// <returns>строка с указанием версий .net</returns>
        public string GetVersionDotNet()
        {
            string res = "";
            using (RegistryKey ndpKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        res = res + versionKeyName + " ";
                    }
                }
            }
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    int releaseKey = (int)ndpKey.GetValue("Release");
                    if (releaseKey >= 393295) { res = res + " v4.6"; }
                    else
                    {
                        if ((releaseKey >= 379893)) { res = res + " v4.5.2"; }
                        else
                        {
                            if ((releaseKey >= 378675)) { res = res + " v4.5.1"; }
                            else
                            {
                                if ((releaseKey >= 378389)) { res = res + " v4.5"; }
                            }
                        }
                    }
                }
            }
            return res.Trim();
        }

        /// <summary>
        /// получает из ветки HKCU/Software/lnl122/ENS значение ключа
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns>значение</returns>
        public string GetValue(string key)
        {
            if (!isReady)
            {
                Init();
            }
            string value = "";
            try
            {
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(HCKU_lev1 + @"\" + HCKU_lev2 + @"\" + HCKU_lev3, true);
                value = rk.GetValue(key).ToString();
                rk.Close();
            }
            catch
            {
                Log.Write("ERROR: не удалось прочитать содержимое ключа реестра " + key);
            }
            return value;
        }

        /// <summary>
        /// получает из ветки HKCU/Software/lnl122/ENS значение ключа - int
        /// </summary>
        /// <param name="key">ключ</param>
        /// <returns>значение</returns>
        public int GetValueInt(string key)
        {
            if (!isReady)
            {
                Init();
            }
            string value = "";
            int res = -1;
            try
            {
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(HCKU_lev1 + @"\" + HCKU_lev2 + @"\" + HCKU_lev3, true);
                value = rk.GetValue(key).ToString();
                rk.Close();
                if (!Int32.TryParse(value, out res))
                {
                    res = -1;
                }
            }
            catch
            {
                Log.Write("ERROR: не удалось прочитать содержимое ключа реестра " + key);
            }
            return res;
        }

        /// <summary>
        /// устанавливает значение в реестре
        /// </summary>
        /// <param name="KeyName">имя ключа</param>
        /// <param name="Value">значение</param>
        public void SetValue(string KeyName, string Value)
        {
            if (!isReady)
            {
                Init();
            }
            try
            {
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(HCKU_lev1 + @"\" + HCKU_lev2 + @"\" + HCKU_lev3, true);
                rk.SetValue(KeyName, Value);
                rk.Close();
            }
            catch
            {
                Log.Write("ERROR: не удалось установить значение для ключа реестра " + KeyName);
            }
        }

        /// <summary>
        /// устанавливает значение в реестре
        /// </summary>
        /// <param name="KeyName">имя ключа</param>
        /// <param name="Value">значение</param>
        public void SetValue(string KeyName, int Value)
        {
            if (!isReady)
            {
                Init();
            }
            try
            {
                RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(HCKU_lev1 + @"\" + HCKU_lev2 + @"\" + HCKU_lev3, true);
                rk.SetValue(KeyName, Value.ToString());
                rk.Close();
            }
            catch
            {
                Log.Write("ERROR: не удалось установить значение int для ключа реестра " + KeyName);
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
