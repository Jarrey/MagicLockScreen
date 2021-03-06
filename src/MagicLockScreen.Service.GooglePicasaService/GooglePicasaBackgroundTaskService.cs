﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_GooglePicasaService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using MagicLockScreen.BackgroundTask;

namespace MagicLockScreen_Service_GooglePicasaService
{
    public class GooglePicasaBackgroundTaskService : BackgroundTaskService
    {
        public GooglePicasaBackgroundTaskService(Service service, XmlElement configXml)
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
            var random = new Random(DateTime.Now.Millisecond);
            var googlePicasaQueryService = Service as GooglePicasaQueryService;
            if (googlePicasaQueryService != null)
            {
                if (parameters.ContainsKey("keyword"))
                {
                    IList<GooglePicasa> googlePicasas = await googlePicasaQueryService.QueryDataAsync(
                        (uint) random.Next(1, (int) googlePicasaQueryService.MaxItemCount),
                        1,
                        parameters["keyword"]);

                    if (googlePicasas != null && googlePicasas.Count > 0)
                    {
                        RandomAccessStreamReference stream =
                            RandomAccessStreamReference.CreateFromUri(new Uri(googlePicasas[0].OriginalImageUrl));
                        await LockScreen.SetImageStreamAsync(await stream.OpenReadAsync());
                    }
                }
            }
        }
    }
}