using System.Collections.Generic;
using System.Runtime.Serialization;
using NoteOne_Core;
using NoteOne_Core.Common.Models;

namespace MagicLockScreen_Helper.Models
{
    [DataContract]
    public class MagicLockScreenServiceChannelModel : ServiceChannelModel
    {
        [DataMember] private bool _hasSecondLogo;

        private IEnumerable<BindableImage> _secondLogo;

        /// <summary>
        ///     Is show overlay in page, defalt is True
        /// </summary>
        [DataMember] private bool _showOverlay = true;

        public MagicLockScreenServiceChannelModel(ServiceChannel channel) : base(channel)
        {
        }

        /// <summary>
        ///     Get and set folder path for local picture library service channel
        /// </summary>
        public string Path { get; protected set; }

        public bool ShowOverlay
        {
            get { return _showOverlay; }
            set { SetProperty(ref _showOverlay, value); }
        }

        public bool HasSecondLogo
        {
            get { return _hasSecondLogo; }
            set { SetProperty(ref _hasSecondLogo, value); }
        }

        public IEnumerable<BindableImage> SecondLogo
        {
            get { return _secondLogo; }
            set { SetProperty(ref _secondLogo, value); }
        }
    }
}