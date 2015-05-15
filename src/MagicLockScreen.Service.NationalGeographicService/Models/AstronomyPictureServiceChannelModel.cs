using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteOne.Core;
using NoteOne.Core.Common.Models;
using MagicLockScreen.Service.AstronomyPictureService.Resources;

namespace MagicLockScreen.Service.AstronomyPictureService.Models
{
    public class AstronomyPictureServiceChannelModel : ServiceChannelModel
    {
        public AstronomyPictureServiceChannelModel(ServiceChannel channel)
            : base(channel)
        {
            Title = ResourcesLoader.Loader["ServiceChannelTitle"];
            SubTitle = ResourcesLoader.Loader["ServiceChannelSubTitle"];
            LogoUri = @"ms-appx:///MagicLockScreen_Service_AstronomyPictureService/Images/Logo.png";
            GroupID = NoteOne.Core.Common.ServiceChannelGroupID.OnlineService;
            PrimaryViewType = typeof(MagicLockScreen.Service.AstronomyPictureService.UI.Views.AstronomyPictureServiceChannelPage);
        }
    }
}
