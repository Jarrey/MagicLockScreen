using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_InterfaceLIFTService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Storage.Streams;
using Windows.System.UserProfile;
using MagicLockScreen.BackgroundTask;

namespace MagicLockScreen_Service_InterfaceLIFTService
{
    public class InterfaceLIFTBackgroundTaskService : BackgroundTaskService
    {
        public InterfaceLIFTBackgroundTaskService(Service service, XmlElement configXml)
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
            var interfaceLIFTQueryService = Service as InterfaceLIFTQueryService;
            if (interfaceLIFTQueryService != null)
            {
                InterfaceLIFT interfaceLIFT =
                    await
                    interfaceLIFTQueryService.QueryDataAsync(
                        (uint) random.Next(0, (int) interfaceLIFTQueryService.MaxItemCount));
                if (interfaceLIFT != null && interfaceLIFT.IsAvailable)
                {
                    RandomAccessStreamReference stream =
                        RandomAccessStreamReference.CreateFromUri(new Uri(interfaceLIFT.OriginalImageUrl));
                    await LockScreen.SetImageStreamAsync(await stream.OpenReadAsync());
                }
            }
        }
    }
}