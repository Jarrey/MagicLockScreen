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
    public class RecommendedAppsChameHOTServiceChannel : RecommendedAppsServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("063FF86B-6461-4B38-90FD-C8A3E44EEDFE")]
        /// </summary>
        public RecommendedAppsChameHOTServiceChannel(XmlElement configXml)
            : base(configXml)
        {
        }

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("063FF86B-6461-4B38-90FD-C8A3E44EEDFE")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            Model = new RecommendedAppsChameHOTServiceChannelModel(this);
        }
    }
}