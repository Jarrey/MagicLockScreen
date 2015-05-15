using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MagicLockScreen_Helper;
using MagicLockScreen_Helper.Resources;
using MagicLockScreen_Service_LocalService.Models;
using NoteOne_Core;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System.UserProfile;
using MagicLockScreen.BackgroundTask;

namespace MagicLockScreen_Service_LocalService
{
    public class LocalPictureLibraryBackgroundTaskService : BackgroundTaskService
    {
        public LocalPictureLibraryBackgroundTaskService(Service service, XmlElement configXml)
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
            var localPictureLibraryQueryService = Service as LocalPictureLibraryQueryService;
            if (localPictureLibraryQueryService != null && parameters != null && parameters.Count > 0)
            {
                string localFolderPath = parameters[ConstKeys.LOCAL_FOLDER_PATH_KEY].Replace('?', ':');
                LocalPictureLibrary localPictureLibrary =
                    await localPictureLibraryQueryService.QueryDataAsync(localFolderPath);
                if (localPictureLibrary != null)
                {
                    try
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(localPictureLibrary.LocalImagePath);
                        await LockScreen.SetImageStreamAsync(await file.OpenReadAsync());
                    }
                    catch (Exception ex)
                    {
                        ex.WriteLog();
                    }
                }
            }
        }
    }
}