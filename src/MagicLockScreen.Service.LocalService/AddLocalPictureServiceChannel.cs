using System;
using MagicLockScreen_Service_LocalService.Models;
using NoteOne_Core;
using Windows.Data.Xml.Dom;

namespace MagicLockScreen_Service_LocalService
{
    public class AddLocalPictureServiceChannel : ServiceChannel
    {
        /// <summary>
        ///     ID is [Guid("394B5D80-66B6-4743-8276-74A473A22B14")]
        /// </summary>
        public AddLocalPictureServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("394B5D80-66B6-4743-8276-74A473A22B14")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            Model = new AddLocalPictureServiceChannelModel(this);
        }
    }
}