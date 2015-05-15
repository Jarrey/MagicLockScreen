using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MagicLockScreen_Helper;
using MagicLockScreen_Service_ImageSearchService.Models;
using MagicLockScreen_Service_ImageSearchService.Resources;
using NoteOne_Core;
using NoteOne_Core.Common;
using NoteOne_Core.Common.Models;
using NoteOne_Core.Interfaces;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Popups;

namespace MagicLockScreen_Service_ImageSearchService
{
    public class BaiduImageSearchServiceChannel : ServiceChannel, ISearchServiceChannel
    {
        private string settingsFileName = "BaiduImageSearchService.setting";

        #region Properties

        public uint LogoCount { get; private set; }

        #endregion

        /// <summary>
        ///     ID is [Guid("C8BAC329-DA5D-4E95-B359-151D75F66A29")]
        /// </summary>
        public BaiduImageSearchServiceChannel(XmlElement configXml) : base(configXml)
        {
        }

        /// <summary>
        ///     Initilize the models from XML config file
        ///     The file is synced via MS Cloud (in RoamingFolder)
        /// </summary>
        public async void InitializeServiceChannelModels()
        {
            if (Models == null) Models = new ObservableCollection<ServiceChannelModel>();
            CleanModels();

            try
            {
                StorageFile settingsFile = null;
                if (await ApplicationData.Current.RoamingFolder.CheckFileExisted(settingsFileName))
                {
                    settingsFile = await ApplicationData.Current.RoamingFolder.GetFileAsync(settingsFileName);
                    XmlDocument xmlSetting = await XmlDocument.LoadFromFileAsync(settingsFile);

                    foreach (XmlElement element in xmlSetting.SelectNodes("/ServiceChannel/Tag"))
                    {
                        AddModel(this, element.GetAttribute("Keyword"), ResourcesLoader.Loader["BaiduSearchSubTitle"]);
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
        /// <param name="queryText"></param>
        /// <param name="subTitle"></param>
        public void AddModel(ServiceChannel serviceChannel, string queryText, string subTitle)
        {
            var model =
                new ImageSearchServiceChannelModel(serviceChannel, queryText, subTitle)
                    {
                        SearchProviderType = SearchProvider.Baidu
                    };

            if (!ContainsModel(model.Title))
            {
                Models.Add(model);

                ServiceChannelGroupModel group =
                    ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[
                        ServiceChannelGroupID.CustomSearchPictures];
                if (group == null)
                {
                    group = new ServiceChannelGroupModel(ServiceChannelGroupID.CustomSearchPictures);
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
        /// <param name="queryText"></param>
        public async Task<bool> RemoveModel(string queryText)
        {
            var dialog = new MessageDialog(ResourcesLoader.Loader["RemoveModelQuestion"]);
            dialog.Commands.Add(new UICommand(ResourcesLoader.Loader["OKButton"], null, 1));
            dialog.Commands.Add(new UICommand(ResourcesLoader.Loader["CancelButton"], null, 0));
            IUICommand command = await dialog.ShowAsync();

            if ((int) command.Id == 0)
                return false;

            for (int i = Models.Count - 1; i >= 0; i--)
            {
                var m = Models[i] as ImageSearchServiceChannelModel;
                if (m != null && m.Title == queryText)
                {
                    Models.Remove(m);
                    ServiceChannelGroupModel group =
                        ServiceChannelManager.CurrentServiceChannelManager.ServiceChannelGroups[
                            ServiceChannelGroupID.CustomSearchPictures];
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
                            ServiceChannelGroupID.CustomSearchPictures];
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
            if (await ApplicationData.Current.RoamingFolder.CheckFileExisted(settingsFileName))
                settingsFile = await ApplicationData.Current.RoamingFolder.GetFileAsync(settingsFileName);
            else
                //InitializeAppSettings, create the setting file and set the default values
                settingsFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync(settingsFileName);

            var xmlSetting = new XmlDocument();

            XmlDocumentFragment xmlDocumentFragment = xmlSetting.CreateDocumentFragment();
            XmlElement rootElement = xmlSetting.CreateElement("ServiceChannel");
            xmlDocumentFragment.AppendChild(rootElement);


            foreach (ServiceChannelModel model in Models)
            {
                XmlElement tagElement = xmlSetting.CreateElement("Tag");
                tagElement.SetAttribute("Keyword", model.Title);
                tagElement.SetAttribute("CreatedDate", DateTime.Now.ToString("yyyyMMdd"));
                rootElement.AppendChild(tagElement);
            }
            xmlSetting.AppendChild(rootElement);
            await xmlSetting.SaveToFileAsync(settingsFile);
        }

        /// <summary>
        ///     Check the model exsits in Models list
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ContainsModel(string queryText)
        {
            foreach (ImageSearchServiceChannelModel m in Models)
            {
                if (m.Title == queryText) return true;
            }
            return false;
        }

        /// <summary>
        ///     Search one specific model in Model list
        /// </summary>
        /// <param name="queryText"></param>
        /// <returns></returns>
        public ServiceChannelModel GetModel(string queryText)
        {
            foreach (ImageSearchServiceChannelModel m in Models)
            {
                if (m.Title == queryText) return m;
            }
            return null;
        }

        protected override void InitializeServiceChannel(XmlElement configXml)
        {
            base.InitializeServiceChannel(configXml);

            if (ID.CompareTo(new Guid("C8BAC329-DA5D-4E95-B359-151D75F66A29")) != 0)
                throw new InvalidOperationException("The ServiceChannel ID is incorrect.");

            LogoCount = configXml.GetAttribute("LogoCount").Check().StringToUInt();

            InitializeServiceChannelModels();

            ApplicationData.Current.DataChanged -= Current_DataChanged;
            ApplicationData.Current.DataChanged += Current_DataChanged;
        }

        public override void InitializeLogo()
        {
            try
            {
                foreach (ImageSearchServiceChannelModel m in Models)
                {
                    InitializeModelLogo(m);
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        /// <summary>
        ///     Initialize the Thumbnail of one model
        /// </summary>
        /// <param name="model"></param>
        public async void InitializeModelLogo(ImageSearchServiceChannelModel model)
        {
            try
            {
                var random = new Random(DateTime.Now.Millisecond);
                var baiduImageSearchService = this["BISS"] as BaiduImageSearchService;
                IList<SearchImage> baiduImages =
                    await
                    baiduImageSearchService.QueryDataAsync(model.Title,
                                                           (uint)
                                                           random.Next(0,
                                                                       (int)
                                                                       (baiduImageSearchService.MaxItemCount - LogoCount)),
                                                           LogoCount);
                if (baiduImages != null && baiduImages.Count > 0)
                {
                    model.Logo = new Collection<BindableImage>(baiduImages as IList<BindableImage>);
                    ApplicationHelper.UpdateTileNotification(baiduImages[0].ThumbnailImageUrl, model.Title,
                                                             baiduImages[0].Title);
                }
            }
            catch (Exception ex)
            {
                ex.WriteLog();
                throw ex;
            }
        }

        private void Current_DataChanged(ApplicationData sender, object args)
        {
            InitializeServiceChannelModels();
        }
    }
}