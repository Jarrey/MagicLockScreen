using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_WWFPictureService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_WWFPictureService
{
    public class WWFPictureServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("66082048-6764-4FB5-82B2-00DC129C079E")]
        /// </summary>
        public WWFPictureServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("66082048-6764-4FB5-82B2-00DC129C079E")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new WWFPictureServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var wwfPictureServiceChannelModel = Model as WWFPictureServiceChannelModel;
            IList<WWFPicture> wwfPictures =
                await (this["WWFPQS"] as WWFPictureQueryService).QueryDataAsync(0U, LogoCount);
            if (wwfPictures != null && wwfPictures.Count > 0)
            {
                wwfPictureServiceChannelModel.Logo =
                    new Collection<BindableImage>(wwfPictures as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(wwfPictures[0].ThumbnailImageUrl,
                                                         wwfPictureServiceChannelModel.Title, wwfPictures[0].Title);
            }
        }
    }
}