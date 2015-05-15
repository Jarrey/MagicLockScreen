using System;
using MagicLockScreen_Service_ImageSearchService.Models;
using NoteOne_Core;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_ImageSearchService
{
    /// <summary>
    ///     ID is [Guid("68EC8334-CD99-4B6A-A14F-832A2309332D")]
    /// </summary>
    public class AddCustomPictureServiceChannel : ServiceChannel
    {
        public AddCustomPictureServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        #region Properties

        #endregion

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("68EC8334-CD99-4B6A-A14F-832A2309332D")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            Model = new AddCustomPictureServiceChannelModel(this);
        }
    }
}