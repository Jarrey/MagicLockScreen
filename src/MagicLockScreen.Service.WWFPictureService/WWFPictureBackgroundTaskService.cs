using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_WWFPictureService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using MagicLockScreen.BackgroundTask;
using MagicLockScreen_Helper;

namespace MagicLockScreen_Service_WWFPictureService
{
    public class WWFPictureBackgroundTaskService : BackgroundTaskService
    {
        public WWFPictureBackgroundTaskService(Service service, XmlElement configXml)
            : base(service, configXml, UpdateLockScreenBackgroundTask.BackgroundTaskSettingFileName)
        {
            DoAsync = Task_Run;
        }

        #region Properties

        public List<object> TimeTriggerTimes { get; private set; }

        #endregion

        protected override void Initialize(XmlElement configXml)
        {
            base.Initialize(configXml);

            TimeTriggerTimes = new List<object>();

            try
            {
                string[] times = configXml.GetAttribute("TimeTriggerTimes").Check("").StringToArray();
                foreach (string time in times)
                {
                    string[] tc = time.StringToArray(':');
                    if (tc.Length > 1)
                        TimeTriggerTimes.Add(new {Name = ResourcesLoader.Loader[tc[0]], Value = tc[1]});
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        private async Task Task_Run(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("LockScreen") && parameters.ContainsKey("Wallpaper"))
            {
                bool updateLockScreen = bool.Parse(parameters["LockScreen"]);
                bool updateWallpaper = bool.Parse(parameters["Wallpaper"]);

                var random = new Random(DateTime.Now.Millisecond);
                var wwfPictureQueryService = Service as WWFPictureQueryService;
                if (wwfPictureQueryService != null)
                {
                    WWFPicture wwfPicture =
                        await
                        wwfPictureQueryService.QueryDataAsync(
                            (uint)random.Next(0, (int)wwfPictureQueryService.MaxItemCount));
                    if (wwfPicture != null && wwfPicture.IsAvailable)
                    {
                        RandomAccessStreamReference stream =
                            RandomAccessStreamReference.CreateFromUri(new Uri(wwfPicture.OriginalImageUrl));

                        if (updateLockScreen)
                        {
                            await LockScreen.SetImageStreamAsync(await stream.OpenReadAsync());
                        }

                        if (updateWallpaper)
                        {
                            await ApplicationHelper.SetWallpaperAsync(await stream.OpenReadAsync(), false);
                        }

                        ApplicationHelper.UpdateTileNotification(wwfPicture.ThumbnailImageUrl,
                                                                 wwfPictureQueryService.ServiceChannel.Model.Title,
                                                                 wwfPicture.Title);
                    }
                }
            }
        }
    }
}