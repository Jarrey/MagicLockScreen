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
    public class RecommendedAppsLoveAppServiceChannel : RecommendedAppsServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("94A2530F-72FF-4B02-8980-449964097AA7")]
        /// </summary>
        public RecommendedAppsLoveAppServiceChannel(XmlElement configXml)
            : base(configXml)
        {
        }

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("94A2530F-72FF-4B02-8980-449964097AA7")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            Model = new RecommendedAppsLoveAppServiceChannelModel(this);
        }
    }
}