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

namespace MagicLockScreen_Service_ImageSearchService
{
    public class InfoImageSearchBackgroundTaskService : BackgroundTaskService
    {
        public InfoImageSearchBackgroundTaskService(Service service, XmlElement configXml)
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
            var infoImageSearchService = Service as InfoImageSearchService;
            if (infoImageSearchService != null)
            {
                SearchImage image =
                    await infoImageSearchService.QueryDataAsync(parameters["keyword"], (uint)random.Next(0, (int)infoImageSearchService.MaxItemCount));
                if (image != null)
                {
                    RandomAccessStreamReference stream =
                        RandomAccessStreamReference.CreateFromUri(new Uri(image.OriginalImageUrl));
                    await LockScreen.SetImageStreamAsync(await stream.OpenReadAsync());
                }
            }
        }
    }
}