using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_WikiMediaPODService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_WikiMediaPODService
{
    public class WikiMediaPODServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("eab06ad9-c2c7-4d85-af55-6378c6dbcbc6")]
        /// </summary>
        public WikiMediaPODServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("eab06ad9-c2c7-4d85-af55-6378c6dbcbc6")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new WikiMediaPODServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var wikiMediaPODServiceChannelModel = Model as WikiMediaPODServiceChannelModel;
            IList<WikiMediaPOD> wikiMediaPODs =
                await (this["WMPQS"] as WikiMediaPODQueryService).QueryDataAsync(LogoCount);
            if (wikiMediaPODs != null && wikiMediaPODs.Count > 0)
            {
                wikiMediaPODServiceChannelModel.Logo =
                    new Collection<BindableImage>(wikiMediaPODs as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(wikiMediaPODs[0].OriginalImageUrl,
                                                         wikiMediaPODServiceChannelModel.Title, wikiMediaPODs[0].Title);
            }
        }
    }
}