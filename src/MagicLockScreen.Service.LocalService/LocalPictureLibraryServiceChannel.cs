using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_LocalService.Models;
using MagicLockScreen_Service_LocalService.Resources;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using NoteOne_Core.Interfaces;
using NoteOne_Utility.Extensions;
using NoteOne_Utility.Helpers;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Popups;
using ApplicationHelper = MagicLockScreen_Helper.ApplicationHelper;

namespace MagicLockScreen_Service_LocalService
{
    public class LocalPictureLibraryServiceChannel : ServiceChannel, ISearchServiceChannel
    {
        private string settingsFileName = "LocalPictureLibraryService.setting";

        /// <summary>
        ///     ID is [Guid("1bc3fb99-e045-4b2d-935d-698bf2f161f2")]
        /// </summary>
        public LocalPictureLibraryServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        /// <summary>
        ///     Initilize the models from XML config file
        /// </summary>
        public async void InitializeServiceChannelModels()
        {
            if (Models == null) Models = new ObservableCollection<ServiceChannelModel>();
            CleanModels();

            try
            {
                StorageFile settingsFile = null;
                if (await ApplicationData.Current.LocalFolder.CheckFileExisted(settingsFileName))
                {
                    settingsFile = await ApplicationData.Current.LocalFolder.GetFileAsync(settingsFileName);
                    XmlDocument xmlSetting = await XmlDocument.LoadFromFileAsync(settingsFile);

                    foreach (XmlElement element in xmlSetting.SelectNodes("/ServiceChannel/Item"))
                    {
                        AddModel(this, element.GetAttribute("Path"));
                    }
                    xmlSetting = null;
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
            }
        }

        /// <summary>
        ///     Add one model into this Models list
        /// </summary>
        /// <param name="serviceChannel"></param>
        /// <param name="path"></param>
        /// <param name="subTitle">for local picture service channel, please ignore this parameter</param>
        public void AddModel(ServiceChannel serviceChannel, string path, string subTitle = "")
        {
            var model =
                new LocalPictureLibraryServiceChannelModel(serviceChannel, path);

            if (!ContainsModel(model.Path))
            {
                Models.Add(model);

                ServiceChannelGroupModel group =
                    ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[
                        ServiceChannelGroupID.LocalPictures];
                if (group == null)
                {
                    group = new ServiceChannelGroupModel(ServiceChannelGroupID.LocalPictures);
                    ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups.AddItem(group);
                }

                if (model.Index > group.Models.Count)
                    group.Models.Add(model);
                else
                    group.Models.Insert(Math.Max(model.Index, 0), model);
                ServiceChannelManager.CurrentServiceChannelManager.AvailableModels.Add(model);

                InitializeModelLogo(model);
            }
        }

        /// <summary>
        ///     Remove one model from this Models list
        /// </summary>
        /// <param name="path"></param>
        public async Task<bool> RemoveModel(string path)
        {
            var dialog = new MessageDialog(ResourcesLoader.Loader["RemoveModelQuestion"]);
            dialog.Commands.Add(new UICommand(ResourcesLoader.Loader["OKButton"], null, 1));
            dialog.Commands.Add(new UICommand(ResourcesLoader.Loader["CancelButton"], null, 0));
            IUICommand command = await dialog.ShowAsync();

            if ((int) command.Id == 0)
                return false;

            for (int i = Models.Count - 1; i >= 0; i--)
            {
                var m = Models[i] as LocalPictureLibraryServiceChannelModel;
                if (m != null && m.Path == path)
                {
                    Models.Remove(m);
                    ServiceChannelGroupModel group =
                        ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[
                            ServiceChannelGroupID.LocalPictures];
                    group.Models.Remove(m);
                    ServiceChannelManager.CurrentServiceChannelManager.AvailableModels.Remove(m);
                    if (group.Models.Count == 0)
                        ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups.Remove(group);
                }
            }
            SaveModels();
            return true;
        }

        /// <summary>
        ///     Clean Models
        /// </summary>
        public void CleanModels()
        {
            if (Models != null)
            {
                foreach (ServiceChannelModel m in Models)
                {
                    ServiceChannelManager.CurrentServiceChannelManager.AvailableModels.Remove(m);
                    ServiceChannelGroupModel group =
                        ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[
                            ServiceChannelGroupID.LocalPictures];
                    group.Models.Remove(m);
                    if (group.Models.Count == 0)
                        ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups.Remove(group);
                }
                Models.Clear();
            }
        }

        /// <summary>
        ///     Dave Models to XMl config file
        /// </summary>
        public async void SaveModels()
        {
            StorageFile settingsFile = null;
            if (await ApplicationData.Current.LocalFolder.CheckFileExisted(settingsFileName))
                settingsFile = await ApplicationData.Current.LocalFolder.GetFileAsync(settingsFileName);
            else
                //InitializeAppSettings, create the setting file and set the default values
                settingsFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(settingsFileName);

            var xmlSetting = new XmlDocument();

            XmlDocumentFragment xmlDocumentFragment = xmlSetting.CreateDocumentFragment();
            XmlElement rootElement = xmlSetting.CreateElement("ServiceChannel");
            xmlDocumentFragment.AppendChild(rootElement);


            foreach (ServiceChannelModel model in Models)
            {
                var m = model as LocalPictureLibraryServiceChannelModel;
                if (m != null)
                {
                    XmlElement tagElement = xmlSetting.CreateElement("Item");
                    tagElement.SetAttribute("Path", m.Path);
                    tagElement.SetAttribute("CreatedDate", DateTime.Now.ToString("yyyyMMdd"));
                    rootElement.AppendChild(tagElement);
                }
            }
            xmlSetting.AppendChild(rootElement);
            await xmlSetting.SaveToFileAsync(settingsFile);
        }

        /// <summary>
        ///     Check the model exsits in Models list
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ContainsModel(string path)
        {
            foreach (LocalPictureLibraryServiceChannelModel m in Models)
            {
                if (m.Path == path) return true;
            }
            return false;
        }

        /// <summary>
        ///     Search one specific model in Model list
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ServiceChannelModel GetModel(string path)
        {
            foreach (LocalPictureLibraryServiceChannelModel m in Models)
            {
                if (m.Path == path) return m;
            }
            return null;
        }

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("1bc3fb99-e045-4b2d-935d-698bf2f161f2")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            InitializeServiceChannelModels();
        }

        /// <summary>
        ///     Initialize the Thumbnail of one model
        /// </summary>
        /// <param name="model"></param>
        public async void InitializeModelLogo(LocalPictureLibraryServiceChannelModel model)
        {
            try
            {
                string localFileName = model.TempLogoFileName;
                string localPath = @"ms-appdata:///temp/" + localFileName;

                using (AsyncLock.Releaser releaser = await ConstKeys.LOCAL_PICTURE_LIBRARY_READ_ASYNC_LOCKER.LockAsync()
                    )
                {
                    LocalPictureLibrary localPictureLibrary =
                        await (this["LPLQS"] as LocalPictureLibraryQueryService).QueryDataAsync(model.Path);
                    if (localPictureLibrary != null)
                    {
                        StorageFile logofile =
                            await StorageFile.GetFileFromPathAsync(localPictureLibrary.LocalImagePath);
                        StorageFile targetLogoFile =
                            await
                            ApplicationData.Current.TemporaryFolder.CreateFileAsync(localFileName,
                                                                                    CreationCollisionOption
                                                                                        .GenerateUniqueName);
                        model.TempLogoFileName = targetLogoFile.Name;
                        await logofile.CopyAndReplaceAsync(targetLogoFile);
                        model.Logo = new Collection<BindableImage> {new BindableImage {ThumbnailImageUrl = localPath}};
                        ApplicationHelper.UpdateTileNotification(localPath, model.Title, localPictureLibrary.Title);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }
    }
}