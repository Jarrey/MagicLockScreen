using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Chameleon.UpdateWallpaper.WinformServiceHost
{

    public class HostSetting
    {
        #region Fields

        private static readonly object FileWriteReadLocker = new object();

        private static HostSetting setting = null;

        #endregion

        #region Constructors

        private HostSetting()
        {
            this.AutoLaunch = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current setting.
        /// </summary>
        public static HostSetting CurrentSetting
        {
            get
            {
                setting = setting ?? ReadSetting();
                return setting;
            }
        }

        /// <summary>
        /// Gets the setting file name.
        /// </summary>
        public static string SettingFileName
        {
            get
            {
                return "config.json";
            }
        }

        /// <summary>
        /// Gets the setting file path.
        /// </summary>
        public static string SettingFilePath
        {
            get
            {
                return Path.Combine(GetCurrentCodePath(), SettingFileName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto-launch with system.
        /// </summary>
        public bool AutoLaunch { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Read setting from local file system.
        /// </summary>
        /// <returns> The <see cref="HostSetting"/>. </returns>
        public static HostSetting ReadSetting()
        {
            if (File.Exists(SettingFilePath))
            {
                lock (FileWriteReadLocker)
                {
                    var json = File.ReadAllText(SettingFilePath, Encoding.Unicode);
                    return JsonConvert.DeserializeObject<HostSetting>(json);
                }
            }
            else
            {
                return new HostSetting();
            }
        }

        /// <summary>
        /// Save setting to local file system.
        /// </summary>
        public static void SaveSetting()
        {
            lock (FileWriteReadLocker)
            {
                var json = JsonConvert.SerializeObject(CurrentSetting);
                File.WriteAllText(SettingFilePath, json, Encoding.Unicode);
            }
        }

        private static string GetCurrentCodePath()
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(dir, @"ChameleonDesk");
        }

        #endregion
    }
}
