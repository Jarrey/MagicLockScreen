using NoteOne_Utility;
using NoteOne_Utility.Helpers;
using Windows.Storage;

namespace MagicLockScreen_Helper
{
    /// <summary>
    ///     For Roaming setting data
    /// </summary>
    public class AppSettings : IAppSettings
    {
        #region For instance

        private static AppSettings _instance;

        // For instance
        private AppSettings()
        {
            Settings = new ObservableDictionary<string, object>();
            Reset();
        }

        public void Reset()
        {
            // Create Settings for AppSettings
            Settings[CURRENT_SEARCH_PROVIDER] = SearchProvider.Baidu.ToString();
            Settings[NEED_HELP] = true;

            // Service feeds status
            Settings[SERVICE_CHANNEL_STATUS_ILS] = true;
            Settings[SERVICE_CHANNEL_STATUS_APS] = true;
            Settings[SERVICE_CHANNEL_STATUS_BIS] = true;
            Settings[SERVICE_CHANNEL_STATUS_FS] = true;
            Settings[SERVICE_CHANNEL_STATUS_GPSC] = false; // Google stops to provide Google Picasa service
            Settings[SERVICE_CHANNEL_STATUS_LPSC] = true;
            Settings[SERVICE_CHANNEL_STATUS_NGSC] = true;
            Settings[SERVICE_CHANNEL_STATUS_OPSC] = true;
            Settings[SERVICE_CHANNEL_STATUS_WMPSC] = true;
            Settings[SERVICE_CHANNEL_STATUS_MSWSC] = true;

            Settings[SERVICE_CHANNEL_STATUS_RALASC] = false; // Stop support show recommended app in EN version

            Settings[ZOOM_FACTORY_INTERVAL] = .2f;

            Settings[LOCAL_IMAGE_SIZE_RANGE] = "MinWidth:800|MinHeight:600|MaxWidth:99999999|MaxHeight:99999999";
        }

        #region Setting fields

        public const string CURRENT_SEARCH_PROVIDER = "CURRENT_SEARCH_PROVIDER";
        public const string NEED_HELP = "NEED_HELP";

        public const string LOCAL_IMAGE_SIZE_RANGE = "LOCAL_IMAGE_SIZE_RANGE";

        #region Keys for online service channel

        public const string SERVICE_CHANNEL_STATUS_ILS = "SERVICE_CHANNEL_STATUS_ILS";
        public const string SERVICE_CHANNEL_STATUS_APS = "SERVICE_CHANNEL_STATUS_APS";
        public const string SERVICE_CHANNEL_STATUS_BIS = "SERVICE_CHANNEL_STATUS_BIS";
        public const string SERVICE_CHANNEL_STATUS_FS = "SERVICE_CHANNEL_STATUS_FS";
        public const string SERVICE_CHANNEL_STATUS_GPSC = "SERVICE_CHANNEL_STATUS_GPSC";
        public const string SERVICE_CHANNEL_STATUS_LPSC = "SERVICE_CHANNEL_STATUS_LPSC";
        public const string SERVICE_CHANNEL_STATUS_NGSC = "SERVICE_CHANNEL_STATUS_NGSC";
        public const string SERVICE_CHANNEL_STATUS_OPSC = "SERVICE_CHANNEL_STATUS_OPSC";
        public const string SERVICE_CHANNEL_STATUS_WMPSC = "SERVICE_CHANNEL_STATUS_WMPSC";
        public const string SERVICE_CHANNEL_STATUS_MSWSC = "SERVICE_CHANNEL_STATUS_MSWSC";

        #region Recommended Apps

        public const string SERVICE_CHANNEL_STATUS_RALASC = "SERVICE_CHANNEL_STATUS_RALASC";

        #endregion

        #endregion

        #region Key for image viewer zoom factory setting

        public const string ZOOM_FACTORY_INTERVAL = "ZOOM_FACTORY_INTERVAL";

        #endregion

        #endregion

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null) _instance = new AppSettings();
                return _instance;
            }
        }

        public object this[string keyName]
        {
            get
            {
                if (Settings.ContainsKey(keyName)) return Settings[keyName];
                else
                    return null;
            }
            set { if (Settings.ContainsKey(keyName)) Settings[keyName] = value; }
        }

        public string SettingFileName
        {
            get { return "MagicLockScreen.AppSettings.setting"; }
        }

        public SettingType Type
        {
            get { return SettingType.Roaming; }
        }

        public StorageFolder SettingFolder
        {
            get { return ApplicationData.Current.RoamingFolder; }
        }

        public ObservableDictionary<string, object> Settings { get; private set; }

        #endregion
    }
}