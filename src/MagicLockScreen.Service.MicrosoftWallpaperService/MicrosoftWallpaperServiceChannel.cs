using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_MicrosoftWallpaperService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_MicrosoftWallpaperService
{
    public class MicrosoftWallpaperServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("793409E5-A79D-4946-8498-674C662B396B")]
        /// </summary>
        public MicrosoftWallpaperServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public DateTime AvailableDate { get; private set; }
        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("793409E5-A79D-4946-8498-674C662B396B")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();
            AvailableDate = configXml.GetAttribute("AvailableDate").Check().StringToDateTime("yyyy/M/dd");

            // For law risk from Microsoft, should hide this channel
            // onle enable this service channel from the config date time
            if (DateTime.Now.Ticks < AvailableDate.Ticks)
            {
                IsEnabled = false;
            }

            Model = new MicrosoftWallpaperServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var random = new Random(DateTime.Now.Millisecond);
            var microsoftWallpaperServiceChannelModel = Model as MicrosoftWallpaperServiceChannelModel;
            IList<MicrosoftWallpaper> microsoftWallpapers = 
                await (this["MSWQS"] as MicrosoftWallpaperQueryService).QueryDataAsync(
                    (uint)random.Next(0, (int)((this["MSWQS"] as MicrosoftWallpaperQueryService).MaxItemCount - LogoCount)), 
                    LogoCount);
            if (microsoftWallpapers != null && microsoftWallpapers.Count > 0)
            {
                microsoftWallpaperServiceChannelModel.Logo = new Collection<BindableImage>(microsoftWallpapers as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(microsoftWallpapers[0].ThumbnailImageUrl,
                                                         microsoftWallpaperServiceChannelModel.Title, microsoftWallpapers[0].Title);
            }
        }
    }
}