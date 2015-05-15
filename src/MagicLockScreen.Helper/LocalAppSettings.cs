using System;
using NoteOne_Utility;
using NoteOne_Utility.Helpers;
using Windows.Storage;

namespace MagicLockScreen_Helper
{
    /// <summary>
    ///     For Local setting data
    /// </summary>
    public class LocalAppSettings : IAppSettings
    {
        #region For instance

        private static LocalAppSettings _instance;

        //For instance
        private LocalAppSettings()
        {
            Settings = new ObservableDictionary<string, object>();
            Reset();
        }

        public void Reset()
        {
            Settings[RECENT_LOCAL_SELECTED_PICTURE_FOLDER_PATH] = "/|";
            Settings[UPGRADE_KEY] = "A1B21ADD-0930-43BD-A88B-9914FC88E0EB";
            Settings[INIT_MAIN_PAGE_COUNT] = 0;
            Settings[LOGO_ANIMATION] = true;
        }

        #region Setting fields

        /// <summary>
        ///     The key for recent local selected picture folder path
        /// </summary>
        public const string RECENT_LOCAL_SELECTED_PICTURE_FOLDER_PATH = "RECENT_LOCAL_SELECTED_PICTURE_FOLDER_PATH";

        /// <summary>
        ///     A key used to cleanup app data after upgrading app
        /// </summary>
        public const string UPGRADE_KEY = "UPGRADE_KEY";

        /// <summary>
        ///     Count main page init times, if it can mod 10, show review and rate prompt
        ///     -1 means never show the prompt
        /// </summary>
        public const string INIT_MAIN_PAGE_COUNT = "INIT_MAIN_PAGE_COUNT";

        public const string LOGO_ANIMATION = "LOGO_ANIMATION";

        #endregion

        public static LocalAppSettings Instance
        {
            get
            {
                if (_instance == null) _instance = new LocalAppSettings();
                return _instance;
            }
        }

        public object this[string keyName]
        {
            get
            {
                if (Settings.ContainsKey(keyName)) return Settings[keyName];
                return null;
            }
            set { if (Settings.ContainsKey(keyName)) Settings[keyName] = value; }
        }

        public string SettingFileName
        {
            get { return "MagicLockScreen.LocalAppSettings.setting"; }
        }

        public SettingType Type
        {
            get { return SettingType.Local; }
        }

        public StorageFolder SettingFolder
        {
            get { return ApplicationData.Current.LocalFolder; }
        }

        public ObservableDictionary<string, object> Settings { get; private set; }

        //Indexer

        #endregion
    }
}