using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_LunarPODService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_LunarPODService
{
    public class LunarPODServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("a859c537-9f8e-44da-960d-6d2fc1fcb84a")]
        /// </summary>
        public LunarPODServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("a859c537-9f8e-44da-960d-6d2fc1fcb84a")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new LunarPODServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var lunarPODServiceChannelModel = Model as LunarPODServiceChannelModel;
            IList<LunarPOD> lunarPODs = await (this["LPQS"] as LunarPODQueryService).QueryDataAsync(LogoCount);
            if (lunarPODs != null && lunarPODs.Count > 0)
            {
                lunarPODServiceChannelModel.Logo = new Collection<BindableImage>(lunarPODs as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(lunarPODs[0].ThumbnailImageUrl,
                                                         lunarPODServiceChannelModel.Title, lunarPODs[0].Title);
            }
        }
    }
}