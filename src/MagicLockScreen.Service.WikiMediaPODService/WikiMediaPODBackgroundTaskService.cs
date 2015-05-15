using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_WikiMediaPODService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using MagicLockScreen.BackgroundTask;

namespace MagicLockScreen_Service_WikiMediaPODService
{
    public class WikiMediaPODBackgroundTaskService : BackgroundTaskService
    {
        public WikiMediaPODBackgroundTaskService(Service service, XmlElement configXml)
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
            var wikiMediaPODQueryService = Service as WikiMediaPODQueryService;
            if (wikiMediaPODQueryService != null)
            {
                WikiMediaPOD wikiMediaPOD = await wikiMediaPODQueryService.QueryDataAsync(
                    DateTime.Now.Subtract(TimeSpan.FromDays(random.Next(0, (int) wikiMediaPODQueryService.MaxItemCount))));
                if (wikiMediaPOD != null && wikiMediaPOD.IsAvailable)
                {
                    RandomAccessStreamReference stream =
                        RandomAccessStreamReference.CreateFromUri(new Uri(wikiMediaPOD.OriginalImageUrl));
                    await LockScreen.SetImageStreamAsync(await stream.OpenReadAsync());
                }
            }
        }
    }
}