using System;
using MagicLockScreen_Service_RecommendationService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Globalization;
using Windows.System.UserProfile;

namespace MagicLockScreen_Service_RecommendationService
{
    public class RecommendedAppsLoveAppServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("94A2530F-72FF-4B02-8980-449964097AA7")]
        /// </summary>
        public RecommendedAppsLoveAppServiceChannel(XmlElement configXml)
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

            if (ID.CompareTo(new Guid("94A2530F-72FF-4B02-8980-449964097AA7")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();
            URL = configXml.GetAttribute("URL").Check();

            // check current region
            string languageTag = GlobalizationPreferences.Languages[0];
            IsEnabled = languageTag.ToLower().Contains(configXml.GetAttribute("Region").Check());

            Model = new RecommendedAppsLoveAppServiceChannelModel(this);
        }
    }
}