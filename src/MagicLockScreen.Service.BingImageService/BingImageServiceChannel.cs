using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_BingImageService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_BingImageService
{
    public class BingImageServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("535de752-8d47-480e-b84e-b3f09e1afbc8")]
        /// </summary>
        public BingImageServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("535de752-8d47-480e-b84e-b3f09e1afbc8")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new BingImageServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var bingImageServiceChannelModel = Model as BingImageServiceChannelModel;
            IList<BingImage> bingImages = await (this["BIQS"] as BingImageQueryService).QueryDataAsync(0U, LogoCount);
            if (bingImages != null && bingImages.Count > 0)
            {
                bingImageServiceChannelModel.Logo = new Collection<BindableImage>(bingImages as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(bingImages[0].ThumbnailImageUrl,
                                                         bingImageServiceChannelModel.Title, bingImages[0].CopyrightLink);
            }
        }
    }
}