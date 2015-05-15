using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_GooglePicasaService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_GooglePicasaService
{
    public class GooglePicasaServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("FEB2C864-0B36-4537-9914-637EC62CCC2C")]
        /// </summary>
        public GooglePicasaServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }
        public string LogoKeyword { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("FEB2C864-0B36-4537-9914-637EC62CCC2C")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();
            LogoKeyword = configXml.GetAttribute("LogoKeyword").Check();

            Model = new GooglePicasaServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var googlePicasaServiceChannelModel = Model as GooglePicasaServiceChannelModel;
            IList<GooglePicasa> googlePicasas =
                await (this["GPQS"] as GooglePicasaQueryService).QueryDataAsync(0U, LogoCount, LogoKeyword);
            if (googlePicasas != null && googlePicasas.Count > 0)
            {
                googlePicasaServiceChannelModel.Logo = new Collection<GooglePicasa>(googlePicasas);
                ApplicationHelper.UpdateTileNotification(googlePicasas[0].OriginalImageUrl,
                                                         googlePicasaServiceChannelModel.Title, googlePicasas[0].Title);
            }
        }
    }
}