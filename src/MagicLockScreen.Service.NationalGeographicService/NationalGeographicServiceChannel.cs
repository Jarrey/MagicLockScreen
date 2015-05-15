using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_NationalGeographicService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_NationalGeographicService
{
    public class NationalGeographicServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("F071A80F-8ABB-4BC8-82E6-1666C82C65C4")]
        /// </summary>
        public NationalGeographicServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("0bf98a7f-7e23-462c-904e-c9f70d7bf59c")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new NationalGeographicServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var nationalGeographicServiceChannelModel = Model as NationalGeographicServiceChannelModel;
            IList<NationalGeographic> nationalGeographics =
                await (this["NGQS"] as NationalGeographicQueryService).QueryDataAsync(LogoCount);
            if (nationalGeographics != null && nationalGeographics.Count > 0)
            {
                nationalGeographicServiceChannelModel.Logo =
                    new Collection<BindableImage>(nationalGeographics as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(nationalGeographics[0].ThumbnailImageUrl,
                                                         nationalGeographicServiceChannelModel.Title,
                                                         nationalGeographics[0].Title);
            }
        }
    }
}