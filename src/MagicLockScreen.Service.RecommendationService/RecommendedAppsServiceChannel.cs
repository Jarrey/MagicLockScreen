using System;
using System.Linq;
using MagicLockScreen_Service_RecommendationService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Globalization;
using Windows.System.UserProfile;

namespace MagicLockScreen_Service_RecommendationService
{
    public class RecommendedAppsServiceChannel : ServiceChannel
    {
        public RecommendedAppsServiceChannel(XmlElement configXml)
            : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }
        public string URL { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();
            URL = configXml.GetAttribute("URL").Check();

            // check current region
            var language = new Language(GlobalizationPreferences.Languages[0]);
            string languageTag = language.LanguageTag.Replace(language.Script + "-", "");
            var regions = configXml.GetAttribute("Region").Check().ToLowerInvariant().StringToArray(',');
            IsEnabled = IsEnabled && (regions.Any(r => r == "all") || regions.Any(r => languageTag.ToLowerInvariant().Contains(r)));
        }
    }
}