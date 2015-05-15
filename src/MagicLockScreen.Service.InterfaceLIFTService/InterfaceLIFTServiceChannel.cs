using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_InterfaceLIFTService.Models;
using NoteOne_Core;
using NoteOne_Core.Common.Models;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_InterfaceLIFTService
{
    public class InterfaceLIFTServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("8910F522-5D9C-4529-A0F9-B24CDE14FA57")]
        /// </summary>
        public InterfaceLIFTServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("8910F522-5D9C-4529-A0F9-B24CDE14FA57")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            Model = new InterfaceLIFTServiceChannelModel(this);
        }

        public override async void InitializeLogo()
        {
            var interfaceLIFTServiceChannelModel = Model as InterfaceLIFTServiceChannelModel;
            IList<InterfaceLIFT> interfaceLIFTs =
                await (this["ILQS"] as InterfaceLIFTQueryService).QueryDataAsync(0U, LogoCount);
            if (interfaceLIFTs != null && interfaceLIFTs.Count > 0)
            {
                interfaceLIFTServiceChannelModel.Logo =
                    new Collection<BindableImage>(interfaceLIFTs as IList<BindableImage>);
                ApplicationHelper.UpdateTileNotification(interfaceLIFTs[0].ThumbnailImageUrl,
                                                         interfaceLIFTServiceChannelModel.Title, interfaceLIFTs[0].Title);
            }
        }
    }
}