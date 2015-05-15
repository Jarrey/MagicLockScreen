using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_ImageSearchService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using MagicLockScreen.BackgroundTask;
using MagicLockScreen_Helper;

namespace MagicLockScreen_Service_ImageSearchService
{
    public class GoogleImageSearchBackgroundTaskService : BackgroundTaskService
    {
        public GoogleImageSearchBackgroundTaskService(Service service, XmlElement configXml)
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
                var googleImageSearchService = Service as GoogleImageSearchService;
                if (googleImageSearchService != null)
                {
                    SearchImage image =
                        await
                        googleImageSearchService.QueryDataAsync(parameters["keyword"],
                                                                random.Next(0, (int)googleImageSearchService.MaxItemCount));
                    if (image != null)
                    {
                        RandomAccessStreamReference stream =
                            RandomAccessStreamReference.CreateFromUri(new Uri(image.OriginalImageUrl));

                        if (updateLockScreen)
                        {
                            await LockScreen.SetImageStreamAsync(await stream.OpenReadAsync());
                        }

                        if (updateWallpaper)
                        {
                            await ApplicationHelper.SetWallpaperAsync(await stream.OpenReadAsync(), false);
                        }

                        ApplicationHelper.UpdateTileNotification(image.OriginalImageUrl,
                                                                 googleImageSearchService.ServiceChannel.Model.Title,
                                                                 image.Title);
                    }
                }
            }
        }
    }
}