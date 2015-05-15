using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_OceanPODService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_OceanPODService
{
    public class OceanPODServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("2BA2015A-D900-4978-AEA1-8C4134E9C6CE")]
        /// </summary>
        public OceanPODServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("2BA2015A-D900-4978-AEA1-8C4134E9C6CE")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new OceanPODServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var oceanPODServiceChannelModel = Model as OceanPODServiceChannelModel;
            IList<OceanPOD> oceanPODs = await (this["OPQS"] as OceanPODQueryService).QueryDataAsync(LogoCount);
            if (oceanPODs != null && oceanPODs.Count > 0)
            {
                oceanPODServiceChannelModel.Logo = new Collection<BindableImage>(oceanPODs as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(oceanPODs[0].ThumbnailImageUrl,
                                                         oceanPODServiceChannelModel.Title, oceanPODs[0].Title);
            }
        }
    }
}