using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace Chameleon.UpdateWallpaper.WinformServiceHost
{
    internal class AutoLaunchRegistryHelper
    {
        private const string KeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private const string KeyName = "Chameleon Desktop Service";

        internal static bool EnableAutoLaunch()
        {
            try
            {
                var path = Path.GetFullPath(Uri.UnescapeDataString(new UriBuilder(Assembly.GetAssembly(typeof(AutoLaunchRegistryHelper)).CodeBase).Path));
                using (var key = Registry.CurrentUser.OpenSubKey(KeyPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue(KeyName, path);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        internal static bool DisableAutoLaunch()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(KeyPath, true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(KeyName, false);
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        internal static bool GetAutoLaunch()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(KeyPath, true))
                {
                    if (key != null)
                    {
                        var value = key.GetValue(KeyName, null);
                        return value != null;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
