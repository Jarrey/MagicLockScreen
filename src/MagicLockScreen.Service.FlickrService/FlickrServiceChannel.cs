using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_FlickrService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_FlickrService
{
    public class FlickrServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("f6e6f65e-95e7-460a-b56f-3bb6b063d2cf")]
        /// </summary>
        public FlickrServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("f6e6f65e-95e7-460a-b56f-3bb6b063d2cf")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new FlickrServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var flickrServiceChannelModel = Model as FlickrServiceChannelModel;
            IList<Flickr> flickrs = await (this["FQS"] as FlickrQueryService).QueryDataAsync(0U, LogoCount);
            if (flickrs != null && flickrs.Count > 0)
            {
                flickrServiceChannelModel.Logo = new Collection<BindableImage>(flickrs as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(flickrs[0].ThumbnailImageUrl, flickrServiceChannelModel.Title,
                                                         flickrs[0].Title);
            }
        }
    }
}