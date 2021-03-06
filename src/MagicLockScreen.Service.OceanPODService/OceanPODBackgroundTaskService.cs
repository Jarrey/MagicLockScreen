﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_OceanPODService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using MagicLockScreen.BackgroundTask;
using MagicLockScreen_Helper;

namespace MagicLockScreen_Service_OceanPODService
{
    public class OceanPODBackgroundTaskService : BackgroundTaskService
    {
        public OceanPODBackgroundTaskService(Service service, XmlElement configXml)
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
                        TimeTriggerTimes.Add(new { Name = ResourcesLoader.Loader[tc[0]], Value = tc[1] });
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
                var oceanPODQueryService = Service as OceanPODQueryService;
                if (oceanPODQueryService != null)
                {
                    OceanPOD oceanPOD = await oceanPODQueryService.QueryDataAsync(
                        DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(0, (int)oceanPODQueryService.MaxItemCount))));
                    if (oceanPOD != null && oceanPOD.IsAvailable)
                    {
                        var stream = RandomAccessStreamReference.CreateFromUri(new Uri(oceanPOD.OriginalImageUrl));

                        if (updateLockScreen)
                        {
                            await LockScreen.SetImageStreamAsync(await stream.OpenReadAsync());
                        }

                        if (updateWallpaper)
                        {
                            await ApplicationHelper.SetWallpaperAsync(await stream.OpenReadAsync(), false);
                        }

                        ApplicationHelper.UpdateTileNotification(oceanPOD.ThumbnailImageUrl,
                                                                 oceanPODQueryService.ServiceChannel.Model.Title,
                                                                 oceanPOD.Title);
                    }
                }
            }
        }
    }
}