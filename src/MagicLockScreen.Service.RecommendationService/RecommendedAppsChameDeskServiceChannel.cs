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
    public class RecommendedAppsChameDeskServiceChannel : RecommendedAppsServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("3FA1336F-1F7E-44CB-8200-7D135BD386AD")]
        /// </summary>
        public RecommendedAppsChameDeskServiceChannel(XmlElement configXml)
            : base(configXml)
        {
        }

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("3FA1336F-1F7E-44CB-8200-7D135BD386AD")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            Model = new RecommendedAppsChameDeskServiceChannelModel(this);
        }
    }
}