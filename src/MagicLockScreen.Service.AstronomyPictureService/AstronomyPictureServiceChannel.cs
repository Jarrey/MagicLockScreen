using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_AstronomyPictureService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_AstronomyPictureService
{
    public class AstronomyPictureServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("F071A80F-8ABB-4BC8-82E6-1666C82C65C4")]
        /// </summary>
        public AstronomyPictureServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("F071A80F-8ABB-4BC8-82E6-1666C82C65C4")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new AstronomyPictureServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var astronomyPictureServiceChannelModel = Model as AstronomyPictureServiceChannelModel;
            IList<AstronomyPicture> astronomyPictures =
                await (this["APQS"] as AstronomyPictureQueryService).QueryDataAsync(LogoCount);
            if (astronomyPictures != null && astronomyPictures.Count > 0)
            {
                astronomyPictureServiceChannelModel.Logo =
                    new Collection<BindableImage>(astronomyPictures as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(astronomyPictures[0].ThumbnailImageUrl,
                                                         astronomyPictureServiceChannelModel.Title,
                                                         astronomyPictures[0].Title);
            }
        }
    }
}